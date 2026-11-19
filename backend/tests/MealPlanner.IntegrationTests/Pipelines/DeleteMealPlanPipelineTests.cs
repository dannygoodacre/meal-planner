using MealPlanner.Application.Commands;
using MealPlanner.Application.Entities;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class DeleteMealPlanPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenPlanExists_DeletesMealsAndIngredientsButNotFoods()
    {
        // Arrange
        Guid food = await Host.SeedFoodAsync("Flour");
        Guid id = await Host.SeedMealPlanAsync("Week 1", ("Dinner", [(food, 500)]));

        // Act
        IResult result = await SendAsync(new DeleteMealPlanCommand(id));

        // Assert
        result.ShouldBeSuccess();

        Assert.Equal(0, await CountMealPlansAsync());
        Assert.Equal(0, await ReadAsync(c => c.Set<MealPlanMeal>().CountAsync(Ct)));
        Assert.Equal(0, await ReadAsync(c => c.Set<MealPlanIngredient>().CountAsync(Ct)));
        Assert.Equal(1, await CountFoodsAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenPlanDoesNotExist_ReturnsNotFound()
    {
        // Act
        IResult result = await SendAsync(new DeleteMealPlanCommand(Guid.NewGuid()));

        // Assert
        result.ShouldBeNotFound();
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult result = await SendAsync(new DeleteMealPlanCommand(Guid.Empty));

        // Assert
        result.ShouldBeInvalid().ShouldHaveError("Id", "Must not be empty.");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
