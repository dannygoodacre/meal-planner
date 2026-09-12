using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

public interface ISearchFoodByName
{
    Task<IResult<SearchResponse<FoodResponse>>> ExecuteAsync(string searchTerm, CancellationToken cancellationToken = default);
}

internal sealed class SearchFoodByNameHandler(ILogger<SearchFoodByNameHandler> logger, IFoodRepository repository)
    : SearchPublicEntityByNameHandler<Food, FoodResponse>(logger, repository), ISearchFoodByName
{
    protected override FoodResponse ToResponse(Food food) => food.ToResponse();
}
