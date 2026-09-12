using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

public interface IGetMealPlan
{
    Task<IResult<MealPlanResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

internal sealed class GetMealPlanHandler(ILogger<GetMealPlanHandler> logger, IMealPlanRepository repository)
    : GetPublicEntityHandler<MealPlan, MealPlanResponse>(logger, repository), IGetMealPlan
{
    protected override MealPlanResponse ToResponse(MealPlan mealPlan) => mealPlan.ToResponse();
}
