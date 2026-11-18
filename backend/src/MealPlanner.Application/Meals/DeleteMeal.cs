using DannyGoodacre.Cqrs;
using DannyGoodacre.Results;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Commands;

public sealed record DeleteMealCommand(Guid Id) : IStateCommand, IPublicIdRequest;

internal sealed class DeleteMeal(IMealRepository mealRepository) : ICommandHandler<DeleteMealCommand>
{
    public async Task<IResult> HandleAsync(DeleteMealCommand command, CancellationToken ct)
    {
        Meal? meal = await mealRepository.GetWithTrackingAsync(command.Id, ct);

        if (meal is null)
        {
            return new NotFound();
        }

        mealRepository.Remove(meal);

        return new Success();
    }
}
