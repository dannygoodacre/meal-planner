using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

public interface IGetPaginatedFoods
{
    Task<IResult<PaginatedPublicEntityResponse<FoodResponse>>> ExecuteAsync(int page, int limit, CancellationToken cancellationToken = default);
}

internal sealed class GetPaginatedFoodsHandler(ILogger<GetPaginatedFoodsHandler> logger, IFoodRepository repository)
    : GetPaginatedPublicEntityHandler<Food, FoodResponse>(logger, repository)
{
    protected private override FoodResponse ToResponse(Food food) => food.ToResponse();
}
