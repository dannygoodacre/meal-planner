using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;
using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class GetMealPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenMealExists_ReturnsMealWithIngredientsAndFoods()
    {
        // Arrange
        Guid food = await Host.SeedFoodAsync("Flour");
        Guid id = await Host.SeedMealAsync("Dough", (food, 500));

        // Act
        MealResponse meal = (await QueryAsync<GetMealQuery, MealResponse>(new GetMealQuery(id))).ShouldBeSuccess();

        // Assert
        Assert.Equal(id, meal.Id);
        Assert.Equal("Dough", meal.Name);

        IngredientResponse ingredient = Assert.Single(meal.Ingredients);

        Assert.Equal(500, ingredient.Quantity);
        Assert.Equal(food, ingredient.Food.Id);
        Assert.Equal("Flour", ingredient.Food.Name);
    }

    [Fact]
    public async Task HandleAsync_WhenMealDoesNotExist_ReturnsNotFound()
    {
        // Act
        IResult<MealResponse> result = await QueryAsync<GetMealQuery, MealResponse>(new GetMealQuery(Guid.NewGuid()));

        // Assert
        result.ShouldBeNotFound();
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult<MealResponse> result = await QueryAsync<GetMealQuery, MealResponse>(new GetMealQuery(Guid.Empty));

        // Assert
        result.ShouldBeInvalid().ShouldHaveError("Id", "Must not be empty.");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
