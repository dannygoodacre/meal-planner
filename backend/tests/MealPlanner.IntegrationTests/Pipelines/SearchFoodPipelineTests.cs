using MealPlanner.Application.Foods;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class SearchFoodPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenTermMatches_ReturnsTheMatches()
    {
        // Arrange
        await Host.SeedFoodAsync("Banana");
        await Host.SeedFoodAsync("Walnut");

        // Act
        SearchResponse<FoodResponse> response = (await QueryAsync<SearchFoodQuery, SearchResponse<FoodResponse>>(new SearchFoodQuery("ban"))).ShouldBeSuccess();

        // Assert
        Assert.Equal("ban", response.Query);
        Assert.Equal(1, response.Count);
        Assert.Equal("Banana", Assert.Single(response.Items).Name);
    }

    [Fact]
    public async Task HandleAsync_WhenTermIsTooShort_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult<SearchResponse<FoodResponse>> result = await QueryAsync<SearchFoodQuery, SearchResponse<FoodResponse>>(new SearchFoodQuery("ab"));

        // Assert
        result.ShouldBeInvalid().ShouldHaveErrorCode("SearchTerm", "MinimumLength");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
