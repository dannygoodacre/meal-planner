namespace MealPlanner.Application.Entities;

public sealed class Ingredient
{
    public int FoodId { get; init; }

    public int MealId { get; init; }

    public int Quantity { get; set; }

    public Food Food { get; init; } = null!;

    public Meal Meal { get; init; } = null!;
}
