using MealPlanner.Application.Enums;

namespace MealPlanner.Application.Entities;

public sealed class Food : PublicEntity, IUniqueRegistryItem
{
    public required Unit Unit { get; set; }

    public required int ReferenceQuantity { get; set; }

    public required decimal Calories { get; set; }

    public required decimal Protein { get; set; }

    public required decimal Carbohydrates { get; set; }

    public required decimal Sugar { get; set; }

    public required decimal Fat { get; set; }

    public required decimal SaturatedFat { get; set; }

    public required decimal Fibre { get; set; }

    public required decimal Salt { get; set; }

    public required string Source { get; set; }
}
