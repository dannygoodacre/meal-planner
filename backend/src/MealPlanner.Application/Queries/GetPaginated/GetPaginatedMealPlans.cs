using DannyGoodacre.Primitives;
using MealPlanner.Application;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;
using Microsoft.Extensions.Logging;

namespace MealPlanPlanner.Application.Queries;

public interface IGetPaginatedMealPlans
{
    Task<IResult<PaginatedPublicEntityResponse<MealPlanResponse>>> ExecuteAsync(int page, int limit, CancellationToken cancellationToken = default);
}

internal sealed class GetPaginatedMealPlansHandler(ILogger<GetPaginatedMealPlansHandler> logger, IMealPlanRepository repository)
    : GetPaginatedPublicEntityHandler<MealPlan, MealPlanResponse>(logger, repository)
{
    protected private override MealPlanResponse ToResponse(MealPlan mealPlan) => mealPlan.ToResponse();
}
