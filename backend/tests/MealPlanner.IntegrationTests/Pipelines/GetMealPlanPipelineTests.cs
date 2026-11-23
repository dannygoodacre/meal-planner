using MealPlanner.Application.MealPlans;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class GetMealPlanPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenPlanExists_ReturnsPlanWithMealsIngredientsAndFoods()
    {
        // Arrange
        Guid food = await Host.SeedFoodAsync("Flour");
        Guid id = await Host.SeedMealPlanAsync("Week 1", ("Dinner", [(food, 500)]));

        // Act
        MealPlanResponse plan = (await QueryAsync<GetMealPlanQuery, MealPlanResponse>(new GetMealPlanQuery(id))).ShouldBeSuccess();

        // Assert
        Assert.Equal(id, plan.Id);
        Assert.Equal("Week 1", plan.Name);

        MealResponse meal = Assert.Single(plan.Meals);

        Assert.NotEqual(Guid.Empty, meal.Id);
        Assert.Equal("Dinner", meal.Name);

        IngredientResponse ingredient = Assert.Single(meal.Ingredients);

        Assert.Equal(500, ingredient.Quantity);
        Assert.Equal(food, ingredient.Food.Id);
        Assert.Equal("Flour", ingredient.Food.Name);
    }

    [Fact]
    public async Task HandleAsync_WhenPlanDoesNotExist_ReturnsNotFound()
    {
        // Act
        IResult<MealPlanResponse> result = await QueryAsync<GetMealPlanQuery, MealPlanResponse>(new GetMealPlanQuery(Guid.NewGuid()));

        // Assert
        result.ShouldBeNotFound();
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult<MealPlanResponse> result = await QueryAsync<GetMealPlanQuery, MealPlanResponse>(new GetMealPlanQuery(Guid.Empty));

        // Assert
        result.ShouldBeInvalid().ShouldHaveErrorCode("Id", "NotEmpty");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
