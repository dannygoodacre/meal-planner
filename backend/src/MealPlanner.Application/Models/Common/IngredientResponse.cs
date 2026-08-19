namespace MealPlanner.Application.Models;

public sealed record IngredientResponse
{
    public required int Quantity { get; init; }

    public required FoodResponse Food { get; init; }
}
