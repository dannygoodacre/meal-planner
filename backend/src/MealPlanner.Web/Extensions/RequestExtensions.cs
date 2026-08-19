using MealPlanner.Application.Commands;
using MealPlanner.Web.Models;

namespace MealPlanner.Web;

internal static class RequestExtensions
{
    public static AddFoodCommand ToCommand(this AddFoodRequest request)
        => new()
        {
            Name = request.Name,
            Unit = request.Unit,
            ReferenceQuantity = request.ReferenceQuantity,
            Calories = request.Calories,
            Protein = request.Protein,
            Carbohydrates = request.Carbohydrates,
            Sugar = request.Sugar,
            Fat = request.Fat,
            SaturatedFat = request.SaturatedFat,
            Fibre = request.Fibre,
            Salt = request.Salt,
            Source = request.Source
        };

    public static UpdateFoodCommand ToCommand(this UpdateFoodRequest request, Guid id)
        => new()
        {
            Id = id,
            Name = request.Name,
            Unit = request.Unit,
            ReferenceQuantity = request.ReferenceQuantity,
            Calories = request.Calories,
            Protein = request.Protein,
            Carbohydrates = request.Carbohydrates,
            Sugar = request.Sugar,
            Fat = request.Fat,
            SaturatedFat = request.SaturatedFat,
            Fibre = request.Fibre,
            Salt = request.Salt,
            Source = request.Source
        };

    public static AddMealCommand ToCommand(this AddMealRequest request)
        => new()
        {
            Name = request.Name,
            Ingredients = request.Ingredients
        };

    public static UpdateMealCommand ToCommand(this UpdateMealRequest request, Guid id)
        => new()
        {
            Id = id,
            Name = request.Name,
            Ingredients = request.Ingredients
        };

    public static AddMealPlanCommand ToCommand(this AddMealPlanRequest request)
        => new()
        {
            Name = request.Name,
            Meals = request.Meals,
        };

    public static UpdateMealPlanCommand ToCommand(this UpdateMealPlanRequest request, Guid id)
        => new()
        {
            Id = id,
            Name = request.Name,
            Meals = request.Meals
        };
}
