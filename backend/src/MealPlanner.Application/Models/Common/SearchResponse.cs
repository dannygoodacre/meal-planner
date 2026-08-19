namespace MealPlanner.Application.Models;

public sealed record SearchResponse<TResponse>
{
    public required string Query { get; init; }

    public required int Count { get; init; }

    public required List<TResponse> Items { get; init; }
}
