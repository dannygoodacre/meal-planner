using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;

namespace MealPlanner.Application;

internal static class MealPlanMealExtensions
{

    extension(MealPlanMeal meal)
    {
        public MealResponse ToResponse()
            => new()
            {
                Id = meal.PublicId,
                Name = meal.Name,
                Ingredients = meal.Ingredients.Select(x => x.ToResponse()).ToList()
            };
    }
}
