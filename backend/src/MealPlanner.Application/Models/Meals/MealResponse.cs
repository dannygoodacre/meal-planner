namespace MealPlanner.Application.Models;

public sealed record MealResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public List<IngredientResponse> Ingredients { get; init; } = [];
}
