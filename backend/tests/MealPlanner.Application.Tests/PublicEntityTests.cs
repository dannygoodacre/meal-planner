using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Tests;

public sealed class PublicEntityTests
{
    [Fact]
    public void SetName_UpdatesNormalizedName()
    {
        var meal = new Meal { PublicId = Guid.NewGuid(), Name = "Crème Brûlée" };

        Assert.Equal("creme brulee", meal.NormalizedName);

        meal.Name = "BANANA";

        Assert.Equal("banana", meal.NormalizedName);
    }
}
