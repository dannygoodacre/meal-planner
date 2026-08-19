namespace MealPlanner.Application.Models;

public sealed record MealPlanResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required List<MealResponse> Meals { get; init; }
}
