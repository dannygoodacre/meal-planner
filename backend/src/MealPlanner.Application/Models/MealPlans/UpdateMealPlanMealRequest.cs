using MealPlanner.Application.Commands;

namespace MealPlanner.Application.Models;

public sealed record UpdateMealPlanMealRequest : IMealDetails
{
    /// <summary>
    /// The meal plan meal to update. Null or empty adds a new meal.
    /// </summary>
    public Guid? Id { get; init; }

    public required string Name { get; init; }

    public List<IngredientRequest> Ingredients { get; init; } = [];
}
