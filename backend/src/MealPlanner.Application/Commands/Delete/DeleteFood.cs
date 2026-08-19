using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

public interface IDeleteFood
{
    Task<Result> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

internal sealed class DeleteFoodHandler(ILogger<DeleteFoodHandler> logger,
                                        IStateUnit stateUnit,
                                        IFoodRepository repository)
    : DeletePublicEntityHandler<Food>(logger, stateUnit, repository), IDeleteFood;
