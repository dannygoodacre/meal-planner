using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;

namespace MealPlanner.Application.MealPlans;

public sealed record SearchMealPlanQuery(string SearchTerm) : IQuery<SearchResponse<MealPlanResponse>>, ISearchTermRequest;

internal sealed class SearchMealPlan(IMealPlanRepository mealPlanRepository) : IQueryHandler<SearchMealPlanQuery, SearchResponse<MealPlanResponse>>
{
    public async Task<IResult<SearchResponse<MealPlanResponse>>> HandleAsync(SearchMealPlanQuery query, CancellationToken ct)
    {
        List<MealPlan> mealPlans = await mealPlanRepository.SearchByNameAsync(query.SearchTerm, ct);

        return new Success<SearchResponse<MealPlanResponse>>(new SearchResponse<MealPlanResponse>
        {
            Query = query.SearchTerm,
            Count = mealPlans.Count,
            Items = [.. mealPlans.Select(x => x.ToResponse())]
        });
    }
}
