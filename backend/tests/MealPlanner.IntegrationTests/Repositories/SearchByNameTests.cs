using MealPlanner.Application.Entities;
using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Repositories;

public sealed class SearchByNameTests(PostgresFixture postgres) : RepositoryTestBase(postgres)
{
    private async Task<List<string>> SearchAsync(string kind, string term)
    {
        List<PublicEntity> results = await Host.InScopeAsync(async sp => await Repository(sp, kind).SearchByNameAsync(term, TestContext.Current.CancellationToken));

        return results.Select(x => x.Name).ToList();
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task SearchByNameAsync_WhenTermIsSubstring_ReturnsMatch(string kind)
    {
        // Arrange
        await SeedAsync(kind, "Ripe banana");
        await SeedAsync(kind, "Cheese");

        // Act
        List<string> names = await SearchAsync(kind, "anan");

        // Assert
        Assert.Equal(["Ripe banana"], names);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task SearchByNameAsync_WhenTermHasTypo_ReturnsMatchThroughTrigrams(string kind)
    {
        // Arrange
        await SeedAsync(kind, "Banana");
        await SeedAsync(kind, "Cheese");

        // Act
        List<string> names = await SearchAsync(kind, "bananna");

        // Assert
        Assert.Equal(["Banana"], names);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task SearchByNameAsync_WhenSomeMatchesArePrefixes_ReturnsPrefixMatchFirst(string kind)
    {
        // Arrange
        await SeedAsync(kind, "Ripe banana");
        await SeedAsync(kind, "Dried banana");
        await SeedAsync(kind, "Banana bread");

        // Act
        List<string> names = await SearchAsync(kind, "banana");

        // Assert
        Assert.Equal(3, names.Count);
        Assert.Equal("Banana bread", names[0]);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task SearchByNameAsync_WhenMoreThanTenMatch_ReturnsTen(string kind)
    {
        // Arrange
        for (int i = 0; i < 12; i++)
        {
            await SeedAsync(kind, $"Banana {i:00}");
        }

        // Act
        List<string> names = await SearchAsync(kind, "banana");

        // Assert
        Assert.Equal(10, names.Count);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task SearchByNameAsync_WhenTermDiffersByCaseAndAccents_ReturnsMatch(string kind)
    {
        // Arrange
        await SeedAsync(kind, "Crème brûlée");

        // Act
        List<string> names = await SearchAsync(kind, "CRÈME BRULEE");

        // Assert
        Assert.Equal(["Crème brûlée"], names);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task SearchByNameAsync_WhenNothingMatches_ReturnsEmpty(string kind)
    {
        // Arrange
        await SeedAsync(kind, "Banana");

        // Act
        List<string> names = await SearchAsync(kind, "zzzzzz");

        // Assert
        Assert.Empty(names);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task SearchByNameAsync_WhenTermIsShorterThanThreeCharacters_StillSearches(string kind)
    {
        // Arrange
        // The length rule lives in SearchTermValidator; the repository applies no minimum of its own.
        await SeedAsync(kind, "Rabbit");
        await SeedAsync(kind, "Cheese");

        // Act
        List<string> names = await SearchAsync(kind, "ab");

        // Assert
        Assert.Equal(["Rabbit"], names);
    }
}
