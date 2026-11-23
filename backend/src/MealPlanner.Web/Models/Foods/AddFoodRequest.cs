using MealPlanner.Application.Enums;

namespace MealPlanner.Web.Models;

public record AddFoodRequest
{
    public required string Name { get; init; }

    public required Unit Unit { get; init; }

    public required int ReferenceQuantity { get; init; }

    public required int Calories { get; init; }

    public required decimal Protein { get; init; }

    public required decimal Carbohydrates { get; set; }

    public required decimal Sugar { get; set; }

    public required decimal Fat { get; set; }

    public required decimal SaturatedFat { get; set; }

    public required decimal Fibre { get; set; }

    public required decimal Salt { get; set; }

    public required string Source { get; set; }
}
