using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Models;

public sealed record PaginatedMealsResponse
{
    public required List<Meal> Meals { get; set; }

    public int CurrentPage { get; set; }

    public int TotalMealsCount { get; set; }

    public int TotalPagesCount { get; set; }
}
