using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;

namespace MealPlanner.Application.Queries;

public sealed record GetFoodQuery(Guid Id) : IQuery<FoodResponse>, IPublicIdRequest;

internal sealed class GetFood(IFoodRepository foodRepository) : IQueryHandler<GetFoodQuery, FoodResponse>
{
    public async Task<IResult<FoodResponse>> HandleAsync(GetFoodQuery query, CancellationToken ct)
    {
        Food? food = await foodRepository.GetAsync(query.Id, ct);

        return food is null
            ? new NotFound<FoodResponse>()
            : new Success<FoodResponse>(food.ToResponse());
    }
}
