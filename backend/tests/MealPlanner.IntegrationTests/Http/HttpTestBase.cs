using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MealPlanner.Application.Enums;
using MealPlanner.IntegrationTests.Infrastructure;
using MealPlanner.Web;
using MealPlanner.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MealPlanner.IntegrationTests.Http;

public abstract class HttpTestBase(PostgresFixture postgres) : IntegrationTestBase(postgres)
{
    private readonly WebApplicationFactory<Program> _factory = new();

    private HttpClient? _client;

    protected static CancellationToken Ct => TestContext.Current.CancellationToken;

    // Matches the JsonStringEnumConverter that Program.cs adds.
    protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    /// <summary>
    /// Starts the real app on first use, after InitializeAsync has reset the database.
    /// </summary>
    protected HttpClient Client => _client ??= _factory
        .WithWebHostBuilder(builder => builder
            // Not Development, so appsettings.Development.json and its dev database connection string are never loaded.
            .UseEnvironment("Testing")
            .UseSetting("ConnectionStrings:DefaultConnection", Postgres.ConnectionString))
        .CreateClient();

    public override async ValueTask DisposeAsync()
    {
        _client?.Dispose();

        await _factory.DisposeAsync();

        await base.DisposeAsync();
    }

    protected async Task<T> ReadJsonAsync<T>(HttpResponseMessage response)
        => (await response.Content.ReadFromJsonAsync<T>(Json, Ct))!;

    protected Task<HttpResponseMessage> PostAsync<T>(string uri, T body) => Client.PostAsJsonAsync(uri, body, Json, Ct);

    protected Task<HttpResponseMessage> PutAsync<T>(string uri, T body) => Client.PutAsJsonAsync(uri, body, Json, Ct);

    protected static AddFoodRequest ValidAddFood(string name = "Banana") => new()
    {
        Name = name,
        Unit = Unit.Grams,
        ReferenceQuantity = 100,
        Calories = 89,
        Protein = 1,
        Carbohydrates = 23,
        Sugar = 12,
        Fat = 0.3m,
        SaturatedFat = 0.1m,
        Fibre = 2.6m,
        Salt = 0,
        Source = "https://example.com/banana"
    };
}
