using MealPlanner.Application.Commands;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class UpdateFoodPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenCommandIsValid_PersistsChanges()
    {
        // Arrange
        Guid id = await Host.SeedFoodAsync("Banana");

        // Act
        IResult result = await SendAsync(ValidUpdateFood(id, "Plantain") with { Calories = 122 });

        // Assert
        result.ShouldBeSuccess();

        var saved = await ReadAsync(c => c.Foods.SingleAsync(x => x.PublicId == id, Ct));

        Assert.Equal("Plantain", saved.Name);
        Assert.Equal("plantain", saved.NormalizedName);
        Assert.Equal(122, saved.Calories);
    }

    [Fact]
    public async Task HandleAsync_WhenFoodDoesNotExist_ReturnsNotFound()
    {
        // Act
        IResult result = await SendAsync(ValidUpdateFood(Guid.NewGuid()));

        // Assert
        result.ShouldBeNotFound();
    }

    [Fact]
    public async Task HandleAsync_WhenRenamedToAnotherFoodsName_ReturnsConflictAndDoesNotSave()
    {
        // Arrange
        await Host.SeedFoodAsync("Apple");
        Guid id = await Host.SeedFoodAsync("Banana");

        // Act
        IResult result = await SendAsync(ValidUpdateFood(id, "apple"));

        // Assert
        result.ShouldBeConflict("A food with the same name already exists");

        Assert.Equal("Banana", (await ReadAsync(c => c.Foods.SingleAsync(x => x.PublicId == id, Ct))).Name);
    }

    [Fact]
    public async Task HandleAsync_WhenOnlyTheCaseOfTheNameChanges_Succeeds()
    {
        // Arrange
        Guid id = await Host.SeedFoodAsync("Banana");

        // Act
        IResult result = await SendAsync(ValidUpdateFood(id, "BANANA"));

        // Assert
        result.ShouldBeSuccess();

        Assert.Equal("BANANA", (await ReadAsync(c => c.Foods.SingleAsync(x => x.PublicId == id, Ct))).Name);
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult result = await SendAsync(ValidUpdateFood(Guid.NewGuid(), "") with { SaturatedFat = 5 });

        // Assert
        ValidationState state = result.ShouldBeInvalid();

        Assert.Equal(2, state.Errors.Count);
        state.ShouldHaveError("Name", "Must not be null or empty.");
        state.ShouldHaveError("SaturatedFat", "Must be less than or equal to Fat.");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult result = await SendAsync(ValidUpdateFood(Guid.Empty));

        // Assert
        result.ShouldBeInvalid().ShouldHaveError("Id", "Must not be empty.");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
