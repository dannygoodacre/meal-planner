using MealPlanner.Application.Meals;

namespace MealPlanner.Application.Models;

public sealed record AddMealPlanMealRequest : IMealDetails
{
    public required string Name { get; init; }

    public List<IngredientRequest> Ingredients { get; init; } = [];
}
