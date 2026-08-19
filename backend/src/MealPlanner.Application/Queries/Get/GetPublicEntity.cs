using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

internal sealed record GetPublicEntityQuery : IQuery
{
    public required Guid Id { get; init; }
}

internal abstract partial class GetPublicEntityHandler<TEntity, TResponse>(ILogger logger, IPublicEntityRepository<TEntity> repository)
    : QueryHandler<GetPublicEntityQuery, TResponse>(logger) where TEntity : PublicEntity
{
    protected override string QueryName => $"Get {typeof(TEntity).Name}";

    protected override void Validate(ValidationState state, GetPublicEntityQuery query)
    {
        state.IsNonEmptyGuid(query.Id, nameof(query.Id));
    }

    protected async override Task<Result<TResponse>> InternalExecuteAsync(GetPublicEntityQuery query, CancellationToken cancellationToken = default)
    {
        LogQueryStarted(Logger, QueryName, query.Id);

        TEntity? entity = await repository.GetAsync(query.Id, cancellationToken);

        if (entity is not null)
        {
            return Result.Success(ToResponse(entity));
        }

        LogNotFound(Logger, QueryName, query.Id);

        return Result<TResponse>.NotFound();
    }

    public Task<Result<TResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        => ExecuteAsync(new GetPublicEntityQuery
        {
            Id = id
        }, cancellationToken);

    protected abstract TResponse ToResponse(TEntity entity);

    [LoggerMessage(LogLevel.Information, "Query '{Query}' started with Id '{Id}'.")]
    private static partial void LogQueryStarted(ILogger logger, string query, Guid id);

    [LoggerMessage(LogLevel.Information, "Query '{Query}' could not find entity with Id '{Id}'.")]
    private static partial void LogNotFound(ILogger logger, string query, Guid id);
}
