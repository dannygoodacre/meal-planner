using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application;

internal static class MealPlanExtensions
{
    extension(MealPlan mealPlan)
    {
        public MealPlanResponse ToResponse()
            => new()
            {
                Id = mealPlan.PublicId,
                Name = mealPlan.Name,
                Meals = mealPlan.Meals.Select(x => x.ToResponse()).ToList()
            };
    }
}
