using MealPlanner.Application.Entities;
using MealPlanner.Data;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.IntegrationTests.Repositories;

public sealed class ConstraintTests(PostgresFixture postgres) : RepositoryTestBase(postgres)
{
    private Task<TResult> WithContextAsync<TResult>(Func<ApplicationContext, Task<TResult>> action)
        => Host.InScopeAsync(sp => action(sp.GetRequiredService<ApplicationContext>()));

    [Theory, MemberData(nameof(Kinds))]
    public async Task Save_WhenNormalizedNameIsDuplicated_ThrowsDbUpdateException(string kind)
    {
        // Arrange
        await SeedAsync(kind, "Banana");

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => SeedAsync(kind, "BANANA"));
    }

    [Fact]
    public async Task Delete_WhenMealIngredientUsesFood_ThrowsDbUpdateException()
    {
        // Arrange
        Guid foodId = await Host.SeedFoodAsync("Flour");
        await Host.SeedMealAsync("Bread", (foodId, 100));

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => WithContextAsync(async context =>
        {
            context.Foods.Remove(await context.Foods.AsTracking().SingleAsync(x => x.PublicId == foodId, TestContext.Current.CancellationToken));

            return await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }));
    }

    [Fact]
    public async Task Delete_WhenMealPlanIngredientUsesFood_ThrowsDbUpdateException()
    {
        // Arrange
        Guid foodId = await Host.SeedFoodAsync("Flour");
        await Host.SeedMealPlanAsync("Week", ("Bread", [(foodId, 100)]));

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(() => WithContextAsync(async context =>
        {
            context.Foods.Remove(await context.Foods.AsTracking().SingleAsync(x => x.PublicId == foodId, TestContext.Current.CancellationToken));

            return await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }));
    }

    [Fact]
    public async Task Delete_WhenMealIsDeleted_CascadesToIngredients()
    {
        // Arrange
        Guid mealId = await Host.SeedMealAsync("Bread");

        // Act
        // ExecuteDelete skips EF's client-side cascade, so this exercises the foreign key itself.
        (int ingredients, int foods) = await WithContextAsync(async context =>
        {
            await context.Meals.Where(x => x.PublicId == mealId).ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            return (await context.Set<Ingredient>().CountAsync(TestContext.Current.CancellationToken),
                await context.Foods.CountAsync(TestContext.Current.CancellationToken));
        });

        // Assert
        Assert.Equal(0, ingredients);
        Assert.Equal(1, foods);
    }

    [Fact]
    public async Task Delete_WhenMealPlanIsDeleted_CascadesToMealsAndIngredients()
    {
        // Arrange
        Guid planId = await Host.SeedMealPlanAsync("Week");

        // Act
        (int meals, int ingredients, int foods) = await WithContextAsync(async context =>
        {
            await context.MealPlans.Where(x => x.PublicId == planId).ExecuteDeleteAsync(TestContext.Current.CancellationToken);

            return (await context.Set<MealPlanMeal>().CountAsync(TestContext.Current.CancellationToken),
                await context.Set<MealPlanIngredient>().CountAsync(TestContext.Current.CancellationToken),
                await context.Foods.CountAsync(TestContext.Current.CancellationToken));
        });

        // Assert
        Assert.Equal(0, meals);
        Assert.Equal(0, ingredients);
        Assert.Equal(1, foods);
    }
}
