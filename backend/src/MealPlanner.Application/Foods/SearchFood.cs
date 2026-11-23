using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;
using MealPlanner.Application.Queries;

namespace MealPlanner.Application.Foods;

public sealed record SearchFoodQuery(string SearchTerm) : IQuery<SearchResponse<FoodResponse>>, ISearchTermRequest;

internal sealed class SearchFood(IFoodRepository foodRepository) : IQueryHandler<SearchFoodQuery, SearchResponse<FoodResponse>>
{
    public async Task<IResult<SearchResponse<FoodResponse>>> HandleAsync(SearchFoodQuery query, CancellationToken ct)
    {
        List<Food> foods = await foodRepository.SearchByNameAsync(query.SearchTerm, ct);

        return new Success<SearchResponse<FoodResponse>>(new SearchResponse<FoodResponse>
        {
            Query = query.SearchTerm,
            Count = foods.Count,
            Items = [.. foods.Select(x => x.ToResponse())]
        });
    }
}
