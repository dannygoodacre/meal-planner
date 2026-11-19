using MealPlanner.Application.Commands;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class AddMealPlanPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenCommandIsValid_SavesPlanWithMealsAndIngredients()
    {
        // Arrange
        Guid flour = await Host.SeedFoodAsync("Flour");
        Guid egg = await Host.SeedFoodAsync("Egg");

        var command = new AddMealPlanCommand
        {
            Name = "Week 1",
            Meals =
            [
                new AddMealPlanMealRequest { Name = "Breakfast", Ingredients = [new(egg, 2)] },
                new AddMealPlanMealRequest { Name = "Dinner", Ingredients = [new(flour, 500), new(egg, 3)] }
            ]
        };

        // Act
        IResult<Guid> result = await AddAsync(command);

        // Assert
        Guid id = result.ShouldBeSuccess();

        Assert.Equal("Week 1", (await ReadAsync(c => c.MealPlans.SingleAsync(x => x.PublicId == id, Ct))).Name);

        var meals = await PlanMealsAsync(id);

        Assert.Equal(["Breakfast", "Dinner"], meals.Select(x => x.Name));
        Assert.Equal([(egg, 2)], meals[0].Ingredients);
        Assert.Equal([(egg, 3), (flour, 500)], meals[1].Ingredients);
    }

    [Fact]
    public async Task HandleAsync_WhenFoodDoesNotExist_ReturnsDomainErrorListingTheId()
    {
        // Arrange
        // Regression: the missing-food check used to be reversed.
        Guid missing = Guid.NewGuid();

        var command = new AddMealPlanCommand
        {
            Name = "Week 1",
            Meals = [new AddMealPlanMealRequest { Name = "Dinner", Ingredients = [new(missing, 1)] }]
        };

        // Act
        IResult<Guid> result = await AddAsync(command);

        // Assert
        Assert.Contains(missing.ToString(), result.ShouldBeDomainError());

        Assert.Equal(0, await CountMealPlansAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenNameExists_ReturnsConflictAndDoesNotSave()
    {
        // Arrange
        await Host.SeedMealPlanAsync("Week 1");
        Guid food = await Host.SeedFoodAsync("Flour");

        var command = new AddMealPlanCommand
        {
            Name = "WEEK 1",
            Meals = [new AddMealPlanMealRequest { Name = "Dinner", Ingredients = [new(food, 1)] }]
        };

        // Act
        IResult<Guid> result = await AddAsync(command);

        // Assert
        result.ShouldBeConflict("A meal plan with the same name already exists");

        Assert.Equal(1, await CountMealPlansAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_ShortCircuitsBeforeTheDatabase()
    {
        // Arrange
        var command = new AddMealPlanCommand
        {
            Name = "",
            Meals = [new AddMealPlanMealRequest { Name = "Dinner", Ingredients = [new(Guid.NewGuid(), 0)] }]
        };

        // Act
        IResult<Guid> result = await AddAsync(command);

        // Assert
        ValidationState state = result.ShouldBeInvalid();

        Assert.Equal(2, state.Errors.Count);
        state.ShouldHaveError("Name", "Must not be null or empty.");
        state.ShouldHaveError("Meals[0].Ingredients[0].Quantity", "Must be greater than 0.");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
