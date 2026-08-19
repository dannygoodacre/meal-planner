namespace MealPlanner.Application.Models;

public sealed record AddMealPlanMealRequest
{
    public string? Name { get; init; }

    public Guid? MealId { get; init; }

    public List<IngredientRequest> Ingredients { get; init; } = [];
}
