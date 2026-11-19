using MealPlanner.Application.Commands;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class AddMealPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenCommandIsValid_SavesMealWithIngredients()
    {
        // Arrange
        Guid flour = await Host.SeedFoodAsync("Flour");
        Guid water = await Host.SeedFoodAsync("Water");

        var command = new AddMealCommand { Name = "Dough", Ingredients = [new(flour, 500), new(water, 300)] };

        // Act
        IResult<Guid> result = await AddAsync(command);

        // Assert
        Guid id = result.ShouldBeSuccess();

        Assert.Equal(1, await CountMealsAsync());
        Assert.Equal([(flour, 500), (water, 300)], (await MealIngredientsAsync(id)).OrderByDescending(x => x.Quantity).ToList());
    }

    [Fact]
    public async Task HandleAsync_WhenFoodDoesNotExist_ReturnsDomainErrorListingTheId()
    {
        // Arrange
        Guid missing = Guid.NewGuid();

        // Act
        IResult<Guid> result = await AddAsync(new AddMealCommand { Name = "Dough", Ingredients = [new(missing, 500)] });

        // Assert
        Assert.Contains(missing.ToString(), result.ShouldBeDomainError());

        Assert.Equal(0, await CountMealsAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenNameExists_ReturnsConflictAndDoesNotSave()
    {
        // Arrange
        await Host.SeedMealAsync("Dough");
        Guid food = await Host.SeedFoodAsync("Flour");

        // Act
        IResult<Guid> result = await AddAsync(new AddMealCommand { Name = "DOUGH", Ingredients = [new(food, 500)] });

        // Assert
        result.ShouldBeConflict("A meal with the same name already exists");

        Assert.Equal(1, await CountMealsAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult<Guid> result = await AddAsync(new AddMealCommand { Name = "", Ingredients = [] });

        // Assert
        ValidationState state = result.ShouldBeInvalid();

        Assert.Equal(2, state.Errors.Count);
        state.ShouldHaveError("Name", "Must not be null or empty.");
        state.ShouldHaveError("Ingredients", "Must not be empty.");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
