namespace MealPlanner.Application.Entities;

public sealed class MealPlan : PublicEntity, IUniqueRegistryItem
{
    public ICollection<MealPlanMeal> Meals { get; init; } = [];
}
