using MealPlanner.Application.MealPlans;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class UpdateMealPlanPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    private Guid _flour;
    private Guid _egg;
    private Guid _planId;

    // Seeds "Week 1" with Breakfast (egg 2) and Dinner (flour 500, egg 3).
    private async Task<List<(Guid Id, string Name, List<(Guid FoodId, int Quantity)> Ingredients)>> SeedPlanAsync()
    {
        _flour = await Host.SeedFoodAsync("Flour");
        _egg = await Host.SeedFoodAsync("Egg");
        _planId = await Host.SeedMealPlanAsync("Week 1", ("Breakfast", [(_egg, 2)]), ("Dinner", [(_flour, 500), (_egg, 3)]));

        return await PlanMealsAsync(_planId);
    }

    [Fact]
    public async Task HandleAsync_WhenMealHasNoId_AddsIt()
    {
        // Arrange
        var seeded = await SeedPlanAsync();

        // Act
        IResult result = await SendAsync(new UpdateMealPlanCommand
        {
            Id = _planId,
            Name = "Week 1",
            Meals =
            [
                new UpdateMealPlanMealRequest { Id = seeded[0].Id, Name = "Breakfast", Ingredients = [new(_egg, 2)] },
                new UpdateMealPlanMealRequest { Id = seeded[1].Id, Name = "Dinner", Ingredients = [new(_flour, 500), new(_egg, 3)] },
                new UpdateMealPlanMealRequest { Name = "Supper", Ingredients = [new(_flour, 50)] }
            ]
        });

        // Assert
        result.ShouldBeSuccess();

        var meals = await PlanMealsAsync(_planId);

        Assert.Equal(["Breakfast", "Dinner", "Supper"], meals.Select(x => x.Name));
        Assert.Equal([(_flour, 50)], meals[2].Ingredients);
        Assert.Equal(seeded[0].Id, meals[0].Id);
    }

    [Fact]
    public async Task HandleAsync_WhenMealHasExistingId_ReconcilesNameAndIngredients()
    {
        // Arrange
        var seeded = await SeedPlanAsync();
        Guid butter = await Host.SeedFoodAsync("Butter");

        // Act
        IResult result = await SendAsync(new UpdateMealPlanCommand
        {
            Id = _planId,
            Name = "Week 1",
            Meals =
            [
                new UpdateMealPlanMealRequest { Id = seeded[0].Id, Name = "Breakfast", Ingredients = [new(_egg, 2)] },
                new UpdateMealPlanMealRequest { Id = seeded[1].Id, Name = "Supper", Ingredients = [new(_egg, 4), new(butter, 20)] }
            ]
        });

        // Assert
        result.ShouldBeSuccess();

        var meals = await PlanMealsAsync(_planId);

        Assert.Equal(2, meals.Count);
        Assert.Equal(seeded[1].Id, meals[1].Id);
        Assert.Equal("Supper", meals[1].Name);
        Assert.Equal([(_egg, 4), (butter, 20)], meals[1].Ingredients);
    }

    [Fact]
    public async Task HandleAsync_WhenMealIsLeftOut_RemovesIt()
    {
        // Arrange
        var seeded = await SeedPlanAsync();

        // Act
        IResult result = await SendAsync(new UpdateMealPlanCommand
        {
            Id = _planId,
            Name = "Week 1",
            Meals = [new UpdateMealPlanMealRequest { Id = seeded[0].Id, Name = "Breakfast", Ingredients = [new(_egg, 2)] }]
        });

        // Assert
        result.ShouldBeSuccess();

        Assert.Equal(["Breakfast"], (await PlanMealsAsync(_planId)).Select(x => x.Name));
    }

    [Fact]
    public async Task HandleAsync_WhenPlanDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        await SeedPlanAsync();

        // Act
        IResult result = await SendAsync(new UpdateMealPlanCommand
        {
            Id = Guid.NewGuid(),
            Name = "Week 1",
            Meals = [new UpdateMealPlanMealRequest { Name = "Dinner", Ingredients = [new(_egg, 1)] }]
        });

        // Assert
        result.ShouldBeNotFound();
    }

    [Fact]
    public async Task HandleAsync_WhenRenamedToAnotherPlansName_ReturnsConflict()
    {
        // Arrange
        await SeedPlanAsync();
        await Host.SeedMealPlanAsync("Week 2");

        // Act
        IResult result = await SendAsync(new UpdateMealPlanCommand
        {
            Id = _planId,
            Name = "week 2",
            Meals = [new UpdateMealPlanMealRequest { Name = "Dinner", Ingredients = [new(_egg, 1)] }]
        });

        // Assert
        result.ShouldBeConflict("A meal plan with the same name already exists");
    }

    [Fact]
    public async Task HandleAsync_WhenFoodDoesNotExist_ReturnsDomainError()
    {
        // Arrange
        await SeedPlanAsync();
        Guid missing = Guid.NewGuid();

        // Act
        IResult result = await SendAsync(new UpdateMealPlanCommand
        {
            Id = _planId,
            Name = "Week 1",
            Meals = [new UpdateMealPlanMealRequest { Name = "Dinner", Ingredients = [new(missing, 1)] }]
        });

        // Assert
        Assert.Contains(missing.ToString(), result.ShouldBeDomainError());
    }

    [Fact]
    public async Task HandleAsync_WhenMealIdIsUnknown_ReturnsDomainErrorAndPersistsNothing()
    {
        // Arrange
        var seeded = await SeedPlanAsync();
        Guid unknown = Guid.NewGuid();

        // Act
        // The first meal is reconciled (and the plan renamed) before the unknown ID is found, so only the
        // state decorator's skipped save keeps those tracked changes out of the database.
        IResult result = await SendAsync(new UpdateMealPlanCommand
        {
            Id = _planId,
            Name = "Renamed",
            Meals =
            [
                new UpdateMealPlanMealRequest { Id = seeded[0].Id, Name = "Changed", Ingredients = [new(_flour, 99)] },
                new UpdateMealPlanMealRequest { Id = unknown, Name = "Ghost", Ingredients = [new(_egg, 1)] }
            ]
        });

        // Assert
        Assert.Contains(unknown.ToString(), result.ShouldBeDomainError());

        Assert.Equal("Week 1", (await ReadAsync(c => c.MealPlans.SingleAsync(x => x.PublicId == _planId, Ct))).Name);

        // Tuples holding lists compare by reference, so compare the parts.
        var after = await PlanMealsAsync(_planId);

        Assert.Equal(seeded.Select(x => (x.Id, x.Name)), after.Select(x => (x.Id, x.Name)));
        Assert.Equal(seeded.SelectMany(x => x.Ingredients), after.SelectMany(x => x.Ingredients));
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult result = await SendAsync(new UpdateMealPlanCommand { Id = Guid.NewGuid(), Name = "", Meals = [] });

        // Assert
        ValidationState state = result.ShouldBeInvalid();

        Assert.Equal(2, state.Errors.Count);
        state.ShouldHaveErrorCode("Name", "Required");
        state.ShouldHaveErrorCode("Meals", "NotEmpty");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult result = await SendAsync(new UpdateMealPlanCommand
        {
            Id = Guid.Empty,
            Name = "Week 1",
            Meals = [new UpdateMealPlanMealRequest { Name = "Dinner", Ingredients = [new(Guid.NewGuid(), 1)] }]
        });

        // Assert
        result.ShouldBeInvalid().ShouldHaveErrorCode("Id", "NotEmpty");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
