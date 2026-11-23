using System.Net;
using MealPlanner.Application.Models;
using MealPlanner.IntegrationTests.Infrastructure;
using MealPlanner.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.IntegrationTests.Http;

public sealed class StatusMappingTests(PostgresFixture postgres) : HttpTestBase(postgres)
{
    [Fact]
    public async Task Post_WhenRequestIsValid_ReturnsCreatedWithLocationThatResolves()
    {
        // Act
        HttpResponseMessage response = await PostAsync("/api/foods", ValidAddFood());

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        Guid id = await ReadJsonAsync<Guid>(response);

        Assert.NotNull(response.Headers.Location);
        Assert.EndsWith($"/api/foods/{id}", response.Headers.Location.ToString());

        HttpResponseMessage get = await Client.GetAsync(response.Headers.Location, Ct);

        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        Assert.Equal(id, (await ReadJsonAsync<FoodResponse>(get)).Id);
    }

    [Fact]
    public async Task Post_WhenHandlerValidationFails_ReturnsValidationProblemDetails()
    {
        // Arrange: binds fine, so only the handler's validator rejects it, not MVC's model validation.
        AddFoodRequest request = ValidAddFood() with { Carbohydrates = 1, Sugar = 2 };

        // Act
        HttpResponseMessage response = await PostAsync("/api/foods", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await ReadJsonAsync<ValidationProblemDetails>(response);

        // The body carries each error's message only; ToValidationProblemDetails drops the "LessThanOrEqualTo" code.
        Assert.Equal(["Sugar"], problem.Errors.Keys);
        Assert.Equal(["Must be less than or equal to Carbohydrates."], problem.Errors["Sugar"]);
    }

    [Fact]
    public async Task Get_WhenIdIsUnknown_ReturnsNotFound()
    {
        // Act
        HttpResponseMessage response = await Client.GetAsync($"/api/foods/{Guid.NewGuid()}", Ct);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_WhenNameExistsInDifferentCase_ReturnsConflict()
    {
        // Arrange
        await Host.SeedFoodAsync("Banana");

        // Act
        HttpResponseMessage response = await PostAsync("/api/foods", ValidAddFood("BANANA"));

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("A food with the same name already exists", (await ReadJsonAsync<ProblemDetails>(response)).Detail);
    }

    [Fact]
    public async Task PostMeal_WhenFoodDoesNotExist_ReturnsUnprocessableEntity()
    {
        // Arrange
        Guid missing = Guid.NewGuid();

        // Act
        HttpResponseMessage response = await PostAsync("/api/meals", new AddMealRequest { Name = "Dough", Ingredients = [new(missing, 500)] });

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Contains(missing.ToString(), (await ReadJsonAsync<ProblemDetails>(response)).Detail);
    }
}
