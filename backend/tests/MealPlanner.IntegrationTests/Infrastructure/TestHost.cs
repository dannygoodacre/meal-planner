using MealPlanner.Application;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Enums;
using MealPlanner.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.IntegrationTests.Infrastructure;

/// <summary>
/// Builds the application's services against the shared Postgres container for one test.
/// </summary>
internal sealed class TestHost : IAsyncDisposable
{
    private readonly ServiceProvider _provider;

    public TestHost(string connectionString)
    {
        DatabaseActivity = new DatabaseActivity();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["ConnectionStrings:DefaultConnection"] = connectionString })
            .Build();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddData(configuration);
        services.AddApplication();

        // AddData takes no options, so the interceptor is attached on top of its AddDbContext call.
        services.ConfigureDbContext<ApplicationContext>(options => options.AddInterceptors(DatabaseActivity));

        _provider = services.BuildServiceProvider(validateScopes: true);
    }

    /// <summary>
    /// Records the SQL commands and transactions that reach the database.
    /// </summary>
    public DatabaseActivity DatabaseActivity { get; }

    /// <summary>
    /// Creates a scope. Use one scope for each request, as a web application does.
    /// </summary>
    public AsyncServiceScope CreateScope() => _provider.CreateAsyncScope();

    public async Task<TResult> InScopeAsync<TResult>(Func<IServiceProvider, Task<TResult>> action)
    {
        await using AsyncServiceScope scope = CreateScope();

        return await action(scope.ServiceProvider);
    }

    public async Task<Guid> SeedFoodAsync(
        string name = "Food",
        Unit unit = Unit.Grams,
        int referenceQuantity = 100,
        int calories = 100,
        decimal protein = 10,
        decimal carbohydrates = 10,
        decimal sugar = 5,
        decimal fat = 5,
        decimal saturatedFat = 2,
        decimal fibre = 1,
        decimal salt = 0.1m,
        string source = "https://example.com")
    {
        var food = new Food
        {
            PublicId = Guid.NewGuid(),
            Name = name,
            Unit = unit,
            ReferenceQuantity = referenceQuantity,
            Calories = calories,
            Protein = protein,
            Carbohydrates = carbohydrates,
            Sugar = sugar,
            Fat = fat,
            SaturatedFat = saturatedFat,
            Fibre = fibre,
            Salt = salt,
            Source = source
        };

        await using AsyncServiceScope scope = CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        context.Foods.Add(food);

        await context.SaveChangesAsync();

        return food.PublicId;
    }

    /// <summary>
    /// Seeds a meal. Each ingredient is a food public ID and a quantity. With no ingredients, one new food is seeded for it.
    /// </summary>
    public async Task<Guid> SeedMealAsync(string name = "Meal", params (Guid FoodId, int Quantity)[] ingredients)
    {
        ingredients = await DefaultIngredientsAsync(ingredients);

        await using AsyncServiceScope scope = CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        Dictionary<Guid, int> foodIds = await FoodIdsAsync(context, ingredients);

        var meal = new Meal
        {
            PublicId = Guid.NewGuid(),
            Name = name,
            Ingredients = ingredients.Select(x => new Ingredient { FoodId = foodIds[x.FoodId], Quantity = x.Quantity }).ToList()
        };

        context.Meals.Add(meal);

        await context.SaveChangesAsync();

        return meal.PublicId;
    }

    /// <summary>
    /// Seeds a meal plan. With no meals, one meal with one new food is seeded for it.
    /// </summary>
    public async Task<Guid> SeedMealPlanAsync(string name = "Meal plan", params (string Name, (Guid FoodId, int Quantity)[] Ingredients)[] meals)
    {
        if (meals.Length == 0)
        {
            meals = [("Meal", [])];
        }

        var resolved = new List<(string Name, (Guid FoodId, int Quantity)[] Ingredients)>();

        foreach ((string mealName, (Guid, int)[] ingredients) in meals)
        {
            resolved.Add((mealName, await DefaultIngredientsAsync(ingredients)));
        }

        await using AsyncServiceScope scope = CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        Dictionary<Guid, int> foodIds = await FoodIdsAsync(context, resolved.SelectMany(x => x.Ingredients));

        var plan = new MealPlan
        {
            PublicId = Guid.NewGuid(),
            Name = name,
            Meals = resolved.Select(meal => new MealPlanMeal
            {
                PublicId = Guid.NewGuid(),
                Name = meal.Name,
                Ingredients = meal.Ingredients
                    .Select(x => new MealPlanIngredient { FoodId = foodIds[x.FoodId], Quantity = x.Quantity })
                    .ToList()
            }).ToList()
        };

        context.MealPlans.Add(plan);

        await context.SaveChangesAsync();

        return plan.PublicId;
    }

    public async ValueTask DisposeAsync() => await _provider.DisposeAsync();

    private async Task<(Guid FoodId, int Quantity)[]> DefaultIngredientsAsync((Guid FoodId, int Quantity)[] ingredients)
        => ingredients.Length > 0 ? ingredients : [(await SeedFoodAsync(Guid.NewGuid().ToString()), 100)];

    private static async Task<Dictionary<Guid, int>> FoodIdsAsync(ApplicationContext context, IEnumerable<(Guid FoodId, int Quantity)> ingredients)
    {
        Guid[] publicIds = ingredients.Select(x => x.FoodId).Distinct().ToArray();

        return await context.Foods
            .Where(x => publicIds.Contains(x.PublicId))
            .ToDictionaryAsync(x => x.PublicId, x => x.Id);
    }
}
