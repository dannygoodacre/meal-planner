namespace MealPlanner.Application.Models;

public sealed record PaginatedPublicEntityResponse<TResponse>
{
    public required List<TResponse> Items { get; set; }

    public int CurrentPage { get; set; }

    public int TotalItemsCount { get; set; }

    public int TotalPagesCount { get; set; }
}
