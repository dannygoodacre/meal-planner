using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

public interface IDeleteMealPlan
{
    Task<IResult> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

internal sealed class DeleteMealPlanHandler(ILogger<DeleteMealPlanHandler> logger,
                                            IStateUnit stateUnit,
                                            IMealPlanRepository repository)
    : DeletePublicEntityHandler<MealPlan>(logger, stateUnit, repository), IDeleteMealPlan;
