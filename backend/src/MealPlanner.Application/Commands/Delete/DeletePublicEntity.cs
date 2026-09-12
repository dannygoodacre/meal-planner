using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Commands;

internal sealed record DeletePublicEntityCommand : ICommand
{
    public required Guid Id { get; init; }
}

internal abstract class DeletePublicEntityHandler<TEntity>(ILogger logger,
                                                           IStateUnit stateUnit,
                                                           IPublicEntityRepository<TEntity> repository)
    : StateCommandHandler<DeletePublicEntityCommand>(logger, stateUnit) where TEntity : PublicEntity
{
    protected override string CommandName => $"Delete ${typeof(TEntity).Name}";

    protected override void Validate(ValidationState state, DeletePublicEntityCommand command)
    {
        state.IsNonEmptyGuid(command.Id, nameof(command.Id));
    }

    protected async override Task<IResult> InternalExecuteAsync(DeletePublicEntityCommand command, CancellationToken cancellationToken = default)
    {
        TEntity? entity = await repository.GetWithTrackingAsync(command.Id, cancellationToken);

        if (entity is null)
        {
            return Result.NotFound();
        }

        repository.Remove(entity);

        return Result.Success();
    }

    public Task<IResult> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        => InternalExecuteAsync(new DeletePublicEntityCommand
        {
            Id = id
        }, cancellationToken);
}
