using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;
using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class SearchMealPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenTermMatches_ReturnsTheMatches()
    {
        // Arrange
        await Host.SeedMealAsync("Lasagne");
        await Host.SeedMealAsync("Omelette");

        // Act
        SearchResponse<MealResponse> response = (await QueryAsync<SearchMealQuery, SearchResponse<MealResponse>>(new SearchMealQuery("las"))).ShouldBeSuccess();

        // Assert
        Assert.Equal("las", response.Query);
        Assert.Equal(1, response.Count);
        Assert.Equal("Lasagne", Assert.Single(response.Items).Name);
    }

    [Fact]
    public async Task HandleAsync_WhenTermIsTooShort_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult<SearchResponse<MealResponse>> result = await QueryAsync<SearchMealQuery, SearchResponse<MealResponse>>(new SearchMealQuery("ab"));

        // Assert
        result.ShouldBeInvalid().ShouldHaveError("SearchTerm", "Must be at least 3 characters long.");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
