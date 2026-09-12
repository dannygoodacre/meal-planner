using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application;

internal static class FoodExtensions
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
}
