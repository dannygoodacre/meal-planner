using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Commands;

public sealed record DeleteMealPlanCommand(Guid Id) : IStateCommand, IPublicIdRequest;

internal sealed class DeleteMealPlan(IMealPlanRepository mealPlanRepository) : ICommandHandler<DeleteMealPlanCommand>
{
    public async Task<IResult> HandleAsync(DeleteMealPlanCommand command, CancellationToken ct)
    {
        MealPlan? mealPlan = await mealPlanRepository.GetWithTrackingAsync(command.Id, ct);

        if (mealPlan is null)
        {
            return new NotFound();
        }

        mealPlanRepository.Remove(mealPlan);

        return new Success();
    }
}
