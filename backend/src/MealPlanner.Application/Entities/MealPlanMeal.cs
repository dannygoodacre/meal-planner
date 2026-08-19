namespace MealPlanner.Application.Entities;

public sealed class MealPlanMeal : PublicEntity
{
    public int MealPlanId { get; init; }

    public MealPlan MealPlan { get; init; } = null!;

    public ICollection<MealPlanIngredient> Ingredients { get; init; } = [];
}
