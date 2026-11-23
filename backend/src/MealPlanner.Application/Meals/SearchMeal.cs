using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;

namespace MealPlanner.Application.Meals;

public sealed record SearchMealQuery(string SearchTerm) : IQuery<SearchResponse<MealResponse>>, ISearchTermRequest;

internal sealed class SearchMeal(IMealRepository mealRepository) : IQueryHandler<SearchMealQuery, SearchResponse<MealResponse>>
{
    public async Task<IResult<SearchResponse<MealResponse>>> HandleAsync(SearchMealQuery query, CancellationToken ct)
    {
        List<Meal> meals = await mealRepository.SearchByNameAsync(query.SearchTerm, ct);

        return new Success<SearchResponse<MealResponse>>(new SearchResponse<MealResponse>
        {
            Query = query.SearchTerm,
            Count = meals.Count,
            Items = [.. meals.Select(x => x.ToResponse())]
        });
    }
}
