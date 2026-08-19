using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

public interface IDeleteMeal
{
    Task<Result> ExecuteAsync(Guid id, CancellationToken cancellationToken = default);
}

internal sealed class DeleteMealHandler(ILogger<DeleteMealHandler> logger,
                                        IStateUnit stateUnit,
                                        IMealRepository repository)
    : DeletePublicEntityHandler<Meal>(logger, stateUnit, repository), IDeleteMeal;
