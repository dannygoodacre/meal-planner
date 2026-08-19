using MealPlanner.Application.Enums;

namespace MealPlanner.Application.Models;

public sealed record FoodResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string NormalizedName { get; init; }

    public required Unit Unit { get; init; }

    public required int ReferenceQuantity { get; init; }

    public required decimal Calories { get; init; }

    public required decimal Protein { get; init; }

    public required decimal Carbohydrates { get; init; }

    public required decimal Sugar { get; init; }

    public required decimal Fat { get; init; }

    public required decimal SaturatedFat { get; init; }

    public required decimal Fibre { get; init; }

    public required decimal Salt { get; init; }

    public required string Source { get; init; }
}
