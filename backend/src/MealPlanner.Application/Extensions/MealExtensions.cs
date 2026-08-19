using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;

namespace MealPlanner.Application;

internal static class MealExtensions
{
    extension(Meal meal)
    {
        public MealResponse ToResponse()
            => new()
            {
                Id = meal.PublicId,
                Name = meal.Name,
                Ingredients = meal.Ingredients.Select(x => x.ToResponse()).ToList()
            };

        public MealPlanMeal ToMealPlanMeal()
            => new()
            {
                PublicId = Guid.NewGuid(),
                Name = meal.Name,
                NormalizedName = meal.NormalizedName,
                Ingredients = meal.Ingredients.Select(x => new MealPlanIngredient
                    {
                        FoodId = x.FoodId,
                        Quantity = x.Quantity
                    })
                    .ToList(),
            };
    }
}
