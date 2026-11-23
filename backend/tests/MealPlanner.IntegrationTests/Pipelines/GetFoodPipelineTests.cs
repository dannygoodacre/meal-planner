using MealPlanner.Application.Enums;
using MealPlanner.Application.Foods;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class GetFoodPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenFoodExists_ReturnsTheFood()
    {
        // Arrange
        Guid id = await Host.SeedFoodAsync("Crème Brûlée");

        // Act
        FoodResponse food = (await QueryAsync<GetFoodQuery, FoodResponse>(new GetFoodQuery(id))).ShouldBeSuccess();

        // Assert
        Assert.Equal(new FoodResponse
        {
            Id = id,
            Name = "Crème Brûlée",
            NormalizedName = "creme brulee",
            Unit = Unit.Grams,
            ReferenceQuantity = 100,
            Calories = 100,
            Protein = 10,
            Carbohydrates = 10,
            Sugar = 5,
            Fat = 5,
            SaturatedFat = 2,
            Fibre = 1,
            Salt = 0.1m,
            Source = "https://example.com"
        }, food);
    }

    [Fact]
    public async Task HandleAsync_WhenFoodDoesNotExist_ReturnsNotFound()
    {
        // Act
        IResult<FoodResponse> result = await QueryAsync<GetFoodQuery, FoodResponse>(new GetFoodQuery(Guid.NewGuid()));

        // Assert
        result.ShouldBeNotFound();
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult<FoodResponse> result = await QueryAsync<GetFoodQuery, FoodResponse>(new GetFoodQuery(Guid.Empty));

        // Assert
        result.ShouldBeInvalid().ShouldHaveErrorCode("Id", "NotEmpty");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
