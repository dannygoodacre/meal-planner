using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Commands;
using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Foods;

public sealed record DeleteFoodCommand(Guid Id) : IStateCommand, IPublicIdRequest;

internal sealed class DeleteFood(IFoodRepository foodRepository) : ICommandHandler<DeleteFoodCommand>
{
    public async Task<IResult> HandleAsync(DeleteFoodCommand command, CancellationToken ct)
    {
        Food? food = await foodRepository.GetWithTrackingAsync(command.Id, ct);

        if (food is null)
        {
            return new NotFound();
        }

        if (await foodRepository.IsInUseAsync(command.Id, ct))
        {
            return new Conflict("The food is used by a meal or meal plan");
        }

        foodRepository.Remove(food);

        return new Success();
    }
}
