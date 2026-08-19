namespace MealPlanner.Application.Models;

public sealed record UpdateMealPlanMealRequest
{
    public Guid? Id { get; init; }

    public Guid? TemplateMealId { get; init; }

    public string? Name { get; init; }

    public List<IngredientRequest> Ingredients { get; init; } = [];
}
