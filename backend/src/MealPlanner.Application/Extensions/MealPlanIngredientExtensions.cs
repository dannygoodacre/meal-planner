using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application;

internal static class MealPlanIngredientExtensions
{
    extension(MealPlanIngredient ingredient)
    {
        public IngredientResponse ToResponse()
            => new()
            {
                Quantity = ingredient.Quantity,
                Food = ingredient.Food.ToResponse()
            };
    }
}
