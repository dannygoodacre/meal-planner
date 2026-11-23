namespace MealPlanner.Application.Entities;

public sealed class MealPlanIngredient
{
    public int FoodId { get; init; }

    public int MealPlanMealId { get; init; }

    public int Quantity { get; set; }

    public Food Food { get; init; } = null!;

    public MealPlanMeal MealPlanMeal { get; init; } = null!;
}
