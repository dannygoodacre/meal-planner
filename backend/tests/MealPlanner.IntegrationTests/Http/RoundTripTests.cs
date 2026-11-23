using System.Net;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;
using MealPlanner.Web.Models;

namespace MealPlanner.IntegrationTests.Http;

public sealed class RoundTripTests(PostgresFixture postgres) : HttpTestBase(postgres)
{
    [Fact]
    public async Task Foods_CreateGetUpdateDelete()
    {
        // Create
        HttpResponseMessage created = await PostAsync("/api/foods", ValidAddFood());
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        string uri = $"/api/foods/{await ReadJsonAsync<Guid>(created)}";

        // Get
        FoodResponse food = await GetOkAsync<FoodResponse>(uri);
        Assert.Equal("Banana", food.Name);
        Assert.Equal(0.3m, food.Fat);

        // Update
        UpdateFoodRequest update = new() { Name = "Plantain", Unit = food.Unit, ReferenceQuantity = 100, Calories = 122, Protein = 1.3m,
            Carbohydrates = 32, Sugar = 15, Fat = 0.4m, SaturatedFat = 0.1m, Fibre = 2.3m, Salt = 0, Source = "https://example.com/plantain" };
        Assert.Equal(HttpStatusCode.NoContent, (await PutAsync(uri, update)).StatusCode);

        food = await GetOkAsync<FoodResponse>(uri);
        Assert.Equal("Plantain", food.Name);
        Assert.Equal(122, food.Calories);
        Assert.Equal("https://example.com/plantain", food.Source);

        // Delete
        await DeleteAndAssertGoneAsync(uri);
    }

    [Fact]
    public async Task Meals_CreateGetUpdateDelete()
    {
        // Arrange
        Guid flour = await Host.SeedFoodAsync("Flour");
        Guid water = await Host.SeedFoodAsync("Water");

        // Create
        HttpResponseMessage created = await PostAsync("/api/meals", new AddMealRequest { Name = "Dough", Ingredients = [new(flour, 500)] });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        string uri = $"/api/meals/{await ReadJsonAsync<Guid>(created)}";

        // Get
        MealResponse meal = await GetOkAsync<MealResponse>(uri);
        Assert.Equal("Dough", meal.Name);
        Assert.Equal([(flour, 500)], meal.Ingredients.Select(x => (x.Food.Id, x.Quantity)));

        // Update
        var update = new UpdateMealRequest { Name = "Bread", Ingredients = [new(flour, 400), new(water, 300)] };
        Assert.Equal(HttpStatusCode.NoContent, (await PutAsync(uri, update)).StatusCode);

        meal = await GetOkAsync<MealResponse>(uri);
        Assert.Equal("Bread", meal.Name);
        Assert.Equal([(flour, 400), (water, 300)], meal.Ingredients.Select(x => (x.Food.Id, x.Quantity)).OrderByDescending(x => x.Quantity));

        // Delete
        await DeleteAndAssertGoneAsync(uri);
    }

    [Fact]
    public async Task MealPlans_CreateGetUpdateDelete()
    {
        // Arrange
        Guid oats = await Host.SeedFoodAsync("Oats");
        Guid milk = await Host.SeedFoodAsync("Milk");

        // Create
        var add = new AddMealPlanRequest { Name = "Week 1", Meals = [new AddMealPlanMealRequest { Name = "Porridge", Ingredients = [new(oats, 50)] }] };
        HttpResponseMessage created = await PostAsync("/api/meal-plans", add);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        string uri = $"/api/meal-plans/{await ReadJsonAsync<Guid>(created)}";

        // Get
        MealPlanResponse plan = await GetOkAsync<MealPlanResponse>(uri);
        Assert.Equal("Week 1", plan.Name);
        MealResponse porridge = Assert.Single(plan.Meals);
        Assert.Equal("Porridge", porridge.Name);
        Assert.Equal([(oats, 50)], porridge.Ingredients.Select(x => (x.Food.Id, x.Quantity)));

        // Update: keep the existing meal by ID with new ingredients
        var update = new UpdateMealPlanRequest
        {
            Name = "Week 2",
            Meals = [new UpdateMealPlanMealRequest { Id = porridge.Id, Name = "Milky porridge", Ingredients = [new(oats, 60), new(milk, 200)] }]
        };
        Assert.Equal(HttpStatusCode.NoContent, (await PutAsync(uri, update)).StatusCode);

        plan = await GetOkAsync<MealPlanResponse>(uri);
        Assert.Equal("Week 2", plan.Name);
        MealResponse updated = Assert.Single(plan.Meals);
        Assert.Equal(porridge.Id, updated.Id);
        Assert.Equal("Milky porridge", updated.Name);
        Assert.Equal([(milk, 200), (oats, 60)], updated.Ingredients.Select(x => (x.Food.Id, x.Quantity)).OrderByDescending(x => x.Quantity));

        // Delete
        await DeleteAndAssertGoneAsync(uri);
    }

    private async Task<T> GetOkAsync<T>(string uri)
    {
        HttpResponseMessage response = await Client.GetAsync(uri, Ct);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        return await ReadJsonAsync<T>(response);
    }

    private async Task DeleteAndAssertGoneAsync(string uri)
    {
        Assert.Equal(HttpStatusCode.NoContent, (await Client.DeleteAsync(uri, Ct)).StatusCode);

        Assert.Equal(HttpStatusCode.NotFound, (await Client.GetAsync(uri, Ct)).StatusCode);
    }
}
