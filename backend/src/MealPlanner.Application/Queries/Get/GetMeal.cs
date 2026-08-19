using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

public interface IGetMeal
{
    Task<Result<MealResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

internal sealed class GetMealHandler(ILogger<GetMealHandler> logger, IMealRepository repository)
    : GetPublicEntityHandler<Meal, MealResponse>(logger, repository), IGetMeal
{
    protected override MealResponse ToResponse(Meal meal) => meal.ToResponse();
}
