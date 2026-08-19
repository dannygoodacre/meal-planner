using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

public interface ISearchMealByName
{
    Task<Result<SearchResponse<MealResponse>>> ExecuteAsync(string searchTerm, CancellationToken cancellationToken = default);
}

internal sealed class SearchMealByNameHandler(ILogger<SearchMealByNameHandler> logger, IMealRepository repository)
    : SearchPublicEntityByNameHandler<Meal, MealResponse>(logger, repository), ISearchMealByName
{
    protected override MealResponse ToResponse(Meal food) => food.ToResponse();
}
