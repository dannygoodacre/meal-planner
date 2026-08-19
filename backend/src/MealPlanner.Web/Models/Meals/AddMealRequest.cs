using MealPlanner.Application.Models;

namespace MealPlanner.Web.Models;

public record AddMealRequest
{
    public required string Name { get; init; }

    public required List<IngredientRequest> Ingredients { get; init; } = [];
}
