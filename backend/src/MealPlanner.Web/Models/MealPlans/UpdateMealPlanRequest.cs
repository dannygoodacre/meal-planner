using MealPlanner.Application.Models;

namespace MealPlanner.Web.Models;

public sealed record UpdateMealPlanRequest
{
    public required string Name { get; init; }

    public required List<UpdateMealPlanMealRequest> Meals { get; init; }
}
