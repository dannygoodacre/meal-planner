using MealPlanner.Application.MealPlans;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class SearchMealPlanPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenTermMatches_ReturnsTheMatches()
    {
        // Arrange
        await Host.SeedMealPlanAsync("Summer cut");
        await Host.SeedMealPlanAsync("Winter bulk");

        // Act
        SearchResponse<MealPlanResponse> response = (await QueryAsync<SearchMealPlanQuery, SearchResponse<MealPlanResponse>>(new SearchMealPlanQuery("sum"))).ShouldBeSuccess();

        // Assert
        Assert.Equal("sum", response.Query);
        Assert.Equal(1, response.Count);
        Assert.Equal("Summer cut", Assert.Single(response.Items).Name);
    }

    [Fact]
    public async Task HandleAsync_WhenTermIsTooShort_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult<SearchResponse<MealPlanResponse>> result = await QueryAsync<SearchMealPlanQuery, SearchResponse<MealPlanResponse>>(new SearchMealPlanQuery("ab"));

        // Assert
        result.ShouldBeInvalid().ShouldHaveErrorCode("SearchTerm", "MinimumLength");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
