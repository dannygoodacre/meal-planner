using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Models;

public sealed record PaginatedFoodResponse
{
    public required List<Food> Foods { get; set; }

    public int CurrentPage { get; set; }

    public int TotalFoodsCount { get; set; }

    public int TotalPagesCount { get; set; }
}
