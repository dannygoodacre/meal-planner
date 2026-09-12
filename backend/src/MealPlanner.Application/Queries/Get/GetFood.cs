using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

public interface IGetFood
{
    Task<IResult<FoodResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

internal sealed class GetFoodHandler(ILogger<GetFoodHandler> logger, IFoodRepository repository)
    : GetPublicEntityHandler<Food, FoodResponse>(logger, repository), IGetFood
{
    protected override FoodResponse ToResponse(Food food) => food.ToResponse();
}
