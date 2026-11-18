using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application;

internal static class MealPlanMealExtensions
{
    extension(MealPlanMeal)
    {
        public static MealPlanMeal Create(string name, IEnumerable<MealPlanIngredient> ingredients)
            => new()
            {
                PublicId = Guid.NewGuid(),
                Name = name,
                Ingredients = ingredients.ToList()
            };
    }

    extension(MealPlanMeal meal)
    {
        public MealResponse ToResponse()
            => new()
            {
                Id = meal.PublicId,
                Name = meal.Name,
                Ingredients = meal.Ingredients.Select(x => x.ToResponse()).ToList()
            };

        /// <summary>
        /// Reconcile the ingredients by food: update quantities, remove missing and add new.
        /// </summary>
        public void SetIngredients(IEnumerable<MealPlanIngredient> ingredients)
        {
            Dictionary<int, MealPlanIngredient> incoming = ingredients.ToDictionary(x => x.FoodId);

            foreach (MealPlanIngredient existing in meal.Ingredients.ToList())
            {
                if (incoming.Remove(existing.FoodId, out MealPlanIngredient? match))
                {
                    existing.Quantity = match.Quantity;
                }
                else
                {
                    meal.Ingredients.Remove(existing);
                }
            }

            foreach (MealPlanIngredient ingredient in incoming.Values)
            {
                meal.Ingredients.Add(ingredient);
            }
        }
    }
}
