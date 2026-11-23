using MealPlanner.Application.Meals;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class UpdateMealPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenIngredientsAreRemovedChangedAndAdded_PersistsAll()
    {
        // Arrange
        Guid kept = await Host.SeedFoodAsync("Flour");
        Guid removed = await Host.SeedFoodAsync("Water");
        Guid added = await Host.SeedFoodAsync("Salt");

        Guid id = await Host.SeedMealAsync("Dough", (kept, 500), (removed, 300));

        // Act
        IResult result = await SendAsync(new UpdateMealCommand
        {
            Id = id,
            Name = "Bread dough",
            Ingredients = [new(kept, 600), new(added, 10)]
        });

        // Assert
        result.ShouldBeSuccess();

        Assert.Equal("Bread dough", (await ReadAsync(c => c.Meals.SingleAsync(x => x.PublicId == id, Ct))).Name);
        Assert.Equal([(kept, 600), (added, 10)], (await MealIngredientsAsync(id)).OrderByDescending(x => x.Quantity).ToList());
    }

    [Fact]
    public async Task HandleAsync_WhenMealDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Guid food = await Host.SeedFoodAsync();

        // Act
        IResult result = await SendAsync(new UpdateMealCommand { Id = Guid.NewGuid(), Name = "Dough", Ingredients = [new(food, 1)] });

        // Assert
        result.ShouldBeNotFound();
    }

    [Fact]
    public async Task HandleAsync_WhenRenamedToAnotherMealsName_ReturnsConflict()
    {
        // Arrange
        await Host.SeedMealAsync("Dough");
        Guid id = await Host.SeedMealAsync("Bread");
        Guid food = await Host.SeedFoodAsync("Flour");

        // Act
        IResult result = await SendAsync(new UpdateMealCommand { Id = id, Name = "dough", Ingredients = [new(food, 1)] });

        // Assert
        result.ShouldBeConflict("A meal with the same name already exists");
    }

    [Fact]
    public async Task HandleAsync_WhenFoodDoesNotExist_ReturnsDomainErrorAndDoesNotSave()
    {
        // Arrange
        Guid id = await Host.SeedMealAsync("Dough");
        List<(Guid, int)> before = await MealIngredientsAsync(id);
        Guid missing = Guid.NewGuid();

        // Act
        IResult result = await SendAsync(new UpdateMealCommand { Id = id, Name = "Bread", Ingredients = [new(missing, 1)] });

        // Assert
        Assert.Contains(missing.ToString(), result.ShouldBeDomainError());

        Assert.Equal("Dough", (await ReadAsync(c => c.Meals.SingleAsync(x => x.PublicId == id, Ct))).Name);
        Assert.Equal(before, await MealIngredientsAsync(id));
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult result = await SendAsync(new UpdateMealCommand { Id = Guid.NewGuid(), Name = "", Ingredients = [] });

        // Assert
        ValidationState state = result.ShouldBeInvalid();

        Assert.Equal(2, state.Errors.Count);
        state.ShouldHaveErrorCode("Name", "Required");
        state.ShouldHaveErrorCode("Ingredients", "NotEmpty");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult result = await SendAsync(new UpdateMealCommand { Id = Guid.Empty, Name = "Dough", Ingredients = [new(Guid.NewGuid(), 1)] });

        // Assert
        result.ShouldBeInvalid().ShouldHaveErrorCode("Id", "NotEmpty");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
