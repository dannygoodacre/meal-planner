using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application;

internal static class ResponseExtensions
{
    extension(Food food)
    {
        public FoodResponse ToResponse()
            => new()
            {
                Id = food.PublicId,
                Name = food.Name,
                NormalizedName = food.NormalizedName,
                Unit = food.Unit,
                ReferenceQuantity = food.ReferenceQuantity,
                Calories = food.Calories,
                Protein = food.Protein,
                Carbohydrates = food.Carbohydrates,
                Sugar = food.Sugar,
                Fat = food.Fat,
                SaturatedFat = food.SaturatedFat,
                Fibre = food.Fibre,
                Salt = food.Salt,
                Source = food.Source
            };
    }

    extension(Ingredient ingredient)
    {
        public IngredientResponse ToResponse()
            => new()
            {
                Quantity = ingredient.Quantity,
                Food = ingredient.Food.ToResponse()
            };
    }

    extension(MealPlanIngredient ingredient)
    {
        public IngredientResponse ToResponse()
            => new()
            {
                Quantity = ingredient.Quantity,
                Food = ingredient.Food.ToResponse()
            };
    }

    extension(Meal meal)
    {
        public MealResponse ToResponse()
            => new()
            {
                Id = meal.PublicId,
                Name = meal.Name,
                Ingredients = [.. meal.Ingredients.Select(x => x.ToResponse())]
            };
    }

    extension(MealPlanMeal meal)
    {
        public MealResponse ToResponse()
            => new()
            {
                Id = meal.PublicId,
                Name = meal.Name,
                Ingredients = [.. meal.Ingredients.Select(x => x.ToResponse())]
            };
    }

    extension(MealPlan mealPlan)
    {
        public MealPlanResponse ToResponse()
            => new()
            {
                Id = mealPlan.PublicId,
                Name = mealPlan.Name,
                Meals = [.. mealPlan.Meals.Select(x => x.ToResponse())]
            };
    }
}
