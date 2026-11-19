using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Repositories;

public sealed class ExistsByNameTests(PostgresFixture postgres) : RepositoryTestBase(postgres)
{
    [Theory]
    [InlineData(Food, "BANANA")]
    [InlineData(Food, "Bánana")]
    [InlineData(Meal, "BANANA")]
    [InlineData(Meal, "Bánana")]
    [InlineData(MealPlan, "BANANA")]
    [InlineData(MealPlan, "Bánana")]
    public async Task ExistsByNameAsync_WhenNameDiffersByCaseOrAccent_ReturnsTrue(string kind, string name)
    {
        // Arrange
        await SeedAsync(kind, "Banana");

        // Act
        bool exists = await Host.InScopeAsync(async sp => await Repository(sp, kind).ExistsByNameAsync(name, TestContext.Current.CancellationToken));

        // Assert
        Assert.True(exists);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task ExistsByNameAsync_WhenNameIsUnknown_ReturnsFalse(string kind)
    {
        // Arrange
        await SeedAsync(kind, "Banana");

        // Act
        bool exists = await Host.InScopeAsync(async sp => await Repository(sp, kind).ExistsByNameAsync("Apple", TestContext.Current.CancellationToken));

        // Assert
        Assert.False(exists);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task ExistsByNameAsync_WhenOnlyExcludedEntityHasName_ReturnsFalse(string kind)
    {
        // Arrange
        Guid id = await SeedAsync(kind, "Banana");

        // Act
        // A case-only rename: the name matches itself once normalised.
        bool exists = await Host.InScopeAsync(async sp => await Repository(sp, kind).ExistsByNameAsync("BANANA", id, TestContext.Current.CancellationToken));

        // Assert
        Assert.False(exists);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task ExistsByNameAsync_WhenAnotherEntityHasName_ReturnsTrue(string kind)
    {
        // Arrange
        Guid id = await SeedAsync(kind, "Banana");
        await SeedAsync(kind, "Cherry");

        // Act
        bool exists = await Host.InScopeAsync(async sp => await Repository(sp, kind).ExistsByNameAsync("CHERRY", id, TestContext.Current.CancellationToken));

        // Assert
        Assert.True(exists);
    }
}
