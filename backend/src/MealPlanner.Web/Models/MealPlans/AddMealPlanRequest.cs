using MealPlanner.Application.Models;

namespace MealPlanner.Web.Models;

public sealed record AddMealPlanRequest
{
    public required string Name { get; init; }

    public required List<AddMealPlanMealRequest> Meals { get; init; }
}
