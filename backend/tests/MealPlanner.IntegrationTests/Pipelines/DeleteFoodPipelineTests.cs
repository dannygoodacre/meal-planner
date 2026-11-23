using MealPlanner.Application.Foods;
using MealPlanner.IntegrationTests.Infrastructure;

namespace MealPlanner.IntegrationTests.Pipelines;

public sealed class DeleteFoodPipelineTests(PostgresFixture postgres) : PipelineTestBase(postgres)
{
    [Fact]
    public async Task HandleAsync_WhenFoodIsUnused_DeletesIt()
    {
        // Arrange
        Guid id = await Host.SeedFoodAsync();

        // Act
        IResult result = await SendAsync(new DeleteFoodCommand(id));

        // Assert
        result.ShouldBeSuccess();

        Assert.Equal(0, await CountFoodsAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenFoodDoesNotExist_ReturnsNotFound()
    {
        // Act
        IResult result = await SendAsync(new DeleteFoodCommand(Guid.NewGuid()));

        // Assert
        result.ShouldBeNotFound();
    }

    [Fact]
    public async Task HandleAsync_WhenAMealUsesTheFood_ReturnsConflictAndDoesNotDelete()
    {
        // Arrange
        Guid id = await Host.SeedFoodAsync("Flour");
        await Host.SeedMealAsync("Bread", (id, 100));

        // Act
        IResult result = await SendAsync(new DeleteFoodCommand(id));

        // Assert
        result.ShouldBeConflict("The food is used by a meal or meal plan");

        Assert.Equal(1, await CountFoodsAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenAMealPlanUsesTheFood_ReturnsConflictAndDoesNotDelete()
    {
        // Arrange
        Guid id = await Host.SeedFoodAsync("Flour");
        await Host.SeedMealPlanAsync("Week", ("Bread", [(id, 100)]));

        // Act
        IResult result = await SendAsync(new DeleteFoodCommand(id));

        // Assert
        result.ShouldBeConflict("The food is used by a meal or meal plan");

        Assert.Equal(1, await CountFoodsAsync());
    }

    [Fact]
    public async Task HandleAsync_WhenIdIsEmpty_ShortCircuitsBeforeTheDatabase()
    {
        // Act
        IResult result = await SendAsync(new DeleteFoodCommand(Guid.Empty));

        // Assert
        result.ShouldBeInvalid().ShouldHaveErrorCode("Id", "NotEmpty");
        Assert.Empty(Host.DatabaseActivity.Commands);
    }
}
