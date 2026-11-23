using MealPlanner.Application.Entities;
using MealPlanner.Application.Meals;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class DeleteMealPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenMealExists_DeletesMealAndIngredientsButNotFoods()
    {
        // Arrange
        Guid food = await Host.SeedFoodAsync("Flour");
        Guid id = await Host.SeedMealAsync("Dough", (food, 500));

        // Act
        IResult result = await SendAsync(new DeleteMealCommand(id));

        // Assert
        result.ShouldBeSuccess();

        Assert.Equal(0, await CountMealsAsync());
        Assert.Equal(0, await ReadAsync(c => c.Set<Ingredient>().CountAsync(Ct)));
        Assert.Equal(1, await CountFoodsAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenMealDoesNotExist_ReturnsNotFound()
    {
        // Act
        IResult result = await SendAsync(new DeleteMealCommand(Guid.NewGuid()));

        // Assert
        result.ShouldBeNotFound();
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult result = await SendAsync(new DeleteMealCommand(Guid.Empty));

        // Assert
        result.ShouldBeInvalid().ShouldHaveErrorCode("Id", "NotEmpty");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
