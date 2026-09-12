using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Models;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

public interface ISearchMealPlanByName
{
    Task<IResult<SearchResponse<MealPlanResponse>>> ExecuteAsync(string searchTerm, CancellationToken cancellationToken = default);
}

internal sealed class SearchMealPlanByNameHandler(ILogger<SearchMealPlanByNameHandler> logger, IMealPlanRepository repository)
    : SearchPublicEntityByNameHandler<MealPlan, MealPlanResponse>(logger, repository), ISearchMealPlanByName
{
    protected override MealPlanResponse ToResponse(MealPlan food) => food.ToResponse();
}
