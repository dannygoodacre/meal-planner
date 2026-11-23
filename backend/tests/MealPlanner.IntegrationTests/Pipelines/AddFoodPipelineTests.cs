using MealPlanner.Application.Enums;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class AddFoodPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenCommandIsValid_SavesFood()
    {
        // Act
        IResult<Guid> result = await AddAsync(ValidAddFood());

        // Assert
        Guid id = result.ShouldBeSuccess();

        var saved = await ReadAsync(c => c.Foods.SingleAsync(x => x.PublicId == id, Ct));

        Assert.Equal("Banana", saved.Name);
        Assert.Equal("banana", saved.NormalizedName);
        Assert.Equal(Unit.Grams, saved.Unit);
        Assert.Equal(89, saved.Calories);
        Assert.Equal(0.3m, saved.Fat);
        Assert.Equal("https://example.com/banana", saved.Source);
    }

    [Fact]
    public async Task HandleAsync_WhenNameExistsInDifferentCase_ReturnsConflictAndDoesNotSave()
    {
        // Arrange
        await Host.SeedFoodAsync("Banana");

        // Act
        IResult<Guid> result = await AddAsync(ValidAddFood("BANANA"));

        // Assert
        result.ShouldBeConflict("A food with the same name already exists");

        Assert.Equal(1, await CountFoodsAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenValidationFails_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult<Guid> result = await AddAsync(ValidAddFood("") with { Sugar = 30 });

        // Assert
        ValidationState state = result.ShouldBeInvalid();

        Assert.Equal(2, state.Errors.Count);
        state.ShouldHaveErrorCode("Name", "Required");
        state.ShouldHaveErrorCode("Sugar", "LessThanOrEqualTo");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
