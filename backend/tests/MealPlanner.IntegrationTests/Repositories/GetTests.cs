using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Data;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.IntegrationTests.Repositories;

public sealed class GetTests(PostgresFixture postgres) : RepositoryTestBase(postgres)
{
    [Theory, MemberData(nameof(Kinds))]
    public async Task GetAsync_WhenIdIsUnknown_ReturnsNull(string kind)
    {
        // Arrange
        await SeedAsync(kind, "Banana");

        // Act
        PublicEntity? result = await Host.InScopeAsync(async sp => await Repository(sp, kind).GetAsync(Guid.NewGuid(), TestContext.Current.CancellationToken));

        // Assert
        Assert.Null(result);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task GetAsync_WhenEntityExists_ReturnsUntrackedEntity(string kind)
    {
        // Arrange
        Guid id = await SeedAsync(kind, "Banana");

        // Act
        EntityState state = await Host.InScopeAsync(async sp =>
        {
            PublicEntity entity = (await Repository(sp, kind).GetAsync(id, TestContext.Current.CancellationToken))!;

            return sp.GetRequiredService<ApplicationContext>().Entry(entity).State;
        });

        // Assert
        Assert.Equal(EntityState.Detached, state);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task GetWithTrackingAsync_WhenEntityExists_ReturnsTrackedEntity(string kind)
    {
        // Arrange
        Guid id = await SeedAsync(kind, "Banana");

        // Act
        EntityState state = await Host.InScopeAsync(async sp =>
        {
            PublicEntity entity = (await Repository(sp, kind).GetWithTrackingAsync(id, TestContext.Current.CancellationToken))!;

            return sp.GetRequiredService<ApplicationContext>().Entry(entity).State;
        });

        // Assert
        Assert.Equal(EntityState.Unchanged, state);
    }

    [Theory, MemberData(nameof(Kinds))]
    public async Task GetWithTrackingAsync_WhenIdIsUnknown_ReturnsNull(string kind)
    {
        // Act
        PublicEntity? result = await Host.InScopeAsync(async sp => await Repository(sp, kind).GetWithTrackingAsync(Guid.NewGuid(), TestContext.Current.CancellationToken));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_ForMeal_LoadsIngredientsAndFoods()
    {
        // Arrange
        Guid foodId = await Host.SeedFoodAsync("Flour");
        Guid mealId = await Host.SeedMealAsync("Bread", (foodId, 250));

        // Act
        Meal meal = (await Host.InScopeAsync(async sp => await sp.GetRequiredService<IMealRepository>().GetAsync(mealId, TestContext.Current.CancellationToken)))!;

        // Assert
        Ingredient ingredient = Assert.Single(meal.Ingredients);
        Assert.Equal(250, ingredient.Quantity);
        Assert.Equal("Flour", ingredient.Food.Name);
        Assert.Equal(foodId, ingredient.Food.PublicId);
    }

    [Fact]
    public async Task GetAsync_ForMealPlan_LoadsMealsIngredientsAndFoods()
    {
        // Arrange
        Guid foodId = await Host.SeedFoodAsync("Flour");
        Guid planId = await Host.SeedMealPlanAsync("Week", ("Bread", [(foodId, 250)]));

        // Act
        MealPlan plan = (await Host.InScopeAsync(async sp => await sp.GetRequiredService<IMealPlanRepository>().GetAsync(planId, TestContext.Current.CancellationToken)))!;

        // Assert
        MealPlanMeal meal = Assert.Single(plan.Meals);
        Assert.Equal("Bread", meal.Name);

        MealPlanIngredient ingredient = Assert.Single(meal.Ingredients);
        Assert.Equal(250, ingredient.Quantity);
        Assert.Equal("Flour", ingredient.Food.Name);
        Assert.Equal(foodId, ingredient.Food.PublicId);
    }

    [Fact]
    public async Task IsInUseAsync_WhenFoodIsUnused_ReturnsFalse()
    {
        // Arrange
        Guid foodId = await Host.SeedFoodAsync("Flour");

        // Act
        bool inUse = await Host.InScopeAsync(async sp => await sp.GetRequiredService<IFoodRepository>().IsInUseAsync(foodId, TestContext.Current.CancellationToken));

        // Assert
        Assert.False(inUse);
    }

    [Fact]
    public async Task IsInUseAsync_WhenMealIngredientUsesFood_ReturnsTrue()
    {
        // Arrange
        Guid foodId = await Host.SeedFoodAsync("Flour");
        await Host.SeedMealAsync("Bread", (foodId, 100));

        // Act
        bool inUse = await Host.InScopeAsync(async sp => await sp.GetRequiredService<IFoodRepository>().IsInUseAsync(foodId, TestContext.Current.CancellationToken));

        // Assert
        Assert.True(inUse);
    }

    [Fact]
    public async Task IsInUseAsync_WhenMealPlanIngredientUsesFood_ReturnsTrue()
    {
        // Arrange
        Guid foodId = await Host.SeedFoodAsync("Flour");
        await Host.SeedMealPlanAsync("Week", ("Bread", [(foodId, 100)]));

        // Act
        bool inUse = await Host.InScopeAsync(async sp => await sp.GetRequiredService<IFoodRepository>().IsInUseAsync(foodId, TestContext.Current.CancellationToken));

        // Assert
        Assert.True(inUse);
    }

    [Fact]
    public async Task GetIdMappingAsync_ReturnsOnlyIdsThatExist()
    {
        // Arrange
        Guid first = await Host.SeedFoodAsync("Flour");
        Guid second = await Host.SeedFoodAsync("Sugar");
        await Host.SeedFoodAsync("Salt");

        // Act
        Dictionary<Guid, int> mapping = await Host.InScopeAsync(async sp => await sp.GetRequiredService<IFoodRepository>()
            .GetIdMappingAsync([first, second, Guid.NewGuid()], TestContext.Current.CancellationToken));

        // Assert
        Assert.Equal(2, mapping.Count);
        Assert.Contains(first, mapping.Keys);
        Assert.Contains(second, mapping.Keys);
    }

    [Fact]
    public async Task GetIdMappingAsync_WhenInputIsEmpty_ReturnsEmpty()
    {
        // Arrange
        await Host.SeedFoodAsync("Flour");

        // Act
        Dictionary<Guid, int> mapping = await Host.InScopeAsync(async sp => await sp.GetRequiredService<IFoodRepository>()
            .GetIdMappingAsync([], TestContext.Current.CancellationToken));

        // Assert
        Assert.Empty(mapping);
    }

    [Fact]
    public async Task GetIdMappingAsync_WhenInputHasDuplicateIds_ReturnsEachOnce()
    {
        // Arrange
        Guid id = await Host.SeedFoodAsync("Flour");

        // Act
        Dictionary<Guid, int> mapping = await Host.InScopeAsync(async sp => await sp.GetRequiredService<IFoodRepository>()
            .GetIdMappingAsync([id, id], TestContext.Current.CancellationToken));

        // Assert
        Assert.Single(mapping);
        Assert.Contains(id, mapping.Keys);
    }
}
