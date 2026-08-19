namespace MealPlanner.Application.Entities;

public sealed class Meal : PublicEntity, IUniqueRegistryItem
{
    public ICollection<Ingredient> Ingredients { get; init; } = [];
}
