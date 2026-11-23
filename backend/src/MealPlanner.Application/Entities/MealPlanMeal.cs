namespace MealPlanner.Application.Entities;

public sealed class MealPlanMeal : PublicEntity
{
    public int MealPlanId { get; init; }

    public MealPlan MealPlan { get; init; } = null!;

    public ICollection<MealPlanIngredient> Ingredients { get; init; } = [];

    public static MealPlanMeal Create(string name, IEnumerable<MealPlanIngredient> ingredients)
        => new()
        {
            PublicId = Guid.NewGuid(),
            Name = name,
            Ingredients = [.. ingredients]
        };

    /// <summary>
    /// Reconcile the ingredients by food: update quantities, remove missing and add new.
    /// </summary>
    public void SetIngredients(IEnumerable<MealPlanIngredient> ingredients)
    {
        Dictionary<int, MealPlanIngredient> incoming = ingredients.ToDictionary(x => x.FoodId);

        foreach (MealPlanIngredient existing in Ingredients.ToList())
        {
            if (incoming.Remove(existing.FoodId, out MealPlanIngredient? match))
            {
                existing.Quantity = match.Quantity;
            }
            else
            {
                Ingredients.Remove(existing);
            }
        }

        foreach (MealPlanIngredient ingredient in incoming.Values)
        {
            Ingredients.Add(ingredient);
        }
    }
}
