using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.Queries;

public sealed record GetMealQuery(Guid Id) : IQuery<MealResponse>, IPublicIdRequest;

internal sealed class GetMeal(IMealRepository mealRepository) : IQueryHandler<GetMealQuery, MealResponse>
{
    public async Task<IResult<MealResponse>> HandleAsync(GetMealQuery query, CancellationToken ct)
    {
        Meal? meal = await mealRepository.GetAsync(query.Id, ct);

        return meal is null
            ? new NotFound<MealResponse>()
            : new Success<MealResponse>(meal.ToResponse());
    }
}
