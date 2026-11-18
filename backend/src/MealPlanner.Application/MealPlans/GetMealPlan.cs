using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.Queries;

public sealed record GetMealPlanQuery(Guid Id) : IQuery<MealPlanResponse>, IPublicIdRequest;

internal sealed class GetMealPlan(IMealPlanRepository mealPlanRepository) : IQueryHandler<GetMealPlanQuery, MealPlanResponse>
{
    public async Task<IResult<MealPlanResponse>> HandleAsync(GetMealPlanQuery query, CancellationToken ct)
    {
        MealPlan? mealPlan = await mealPlanRepository.GetAsync(query.Id, ct);

        return mealPlan is null
            ? new NotFound<MealPlanResponse>()
            : new Success<MealPlanResponse>(mealPlan.ToResponse());
    }
}
