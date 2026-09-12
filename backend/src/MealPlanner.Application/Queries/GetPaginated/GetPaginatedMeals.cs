using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

public interface IGetPaginatedMeals
{
    Task<IResult<PaginatedPublicEntityResponse<MealResponse>>> ExecuteAsync(int page, int limit, CancellationToken cancellationToken = default);
}

internal sealed class GetPaginatedMealsHandler(ILogger<GetPaginatedMealsHandler> logger, IMealRepository repository)
    : GetPaginatedPublicEntityHandler<Meal, MealResponse>(logger, repository)
{
    protected private override MealResponse ToResponse(Meal meal) => meal.ToResponse();
}
