using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

internal sealed record GetPaginatedPublicEntityQuery : IQuery
{
    public required int Page { get; init; }

    public required int Limit { get; init; }
}

internal abstract partial class GetPaginatedPublicEntityHandler<TEntity, TResponse>(ILogger logger, IPublicEntityRepository<TEntity> repository)
    : QueryHandler<GetPaginatedPublicEntityQuery, PaginatedPublicEntityResponse<TResponse>>(logger) where TEntity : PublicEntity
{
    protected override string QueryName => $"Get Paginated {typeof(TEntity).Name}s";

    protected override void Validate(ValidationState state, GetPaginatedPublicEntityQuery query)
    {
        state.IsNonNegative(query.Page, nameof(query.Page));

        state.IsPositive(query.Limit, nameof(query.Limit));
    }

    protected async override Task<IResult<PaginatedPublicEntityResponse<TResponse>>> InternalExecuteAsync(GetPaginatedPublicEntityQuery query, CancellationToken cancellationToken = default)
    {
        LogQueryStarted(Logger, QueryName, query.Page, query.Limit);

        int count = await repository.GetCountAsync(cancellationToken);

        List<TEntity> entities = await repository.GetPaginatedAsync(query.Page, query.Limit, cancellationToken);

        return Result.Success(new PaginatedPublicEntityResponse<TResponse>()
        {
            Items = entities.Select(ToResponse).ToList(),

            CurrentPage = query.Page,

            TotalItemsCount = count,

            TotalPagesCount = (count + query.Limit - 1) / query.Limit
        });
    }

    public Task<IResult<PaginatedPublicEntityResponse<TResponse>>> ExecuteAsync(int page, int limit, CancellationToken cancellationToken = default)
        => ExecuteAsync(new GetPaginatedPublicEntityQuery
        {
            Page = page,
            Limit = limit
        }, cancellationToken);

    protected private abstract TResponse ToResponse(TEntity entity);

    [LoggerMessage(LogLevel.Information, "Query '{Query}' started with Page '{Page}' and Limit '{Limit}'.")]
    private static partial void LogQueryStarted(ILogger logger, string query, int page, int limit);
}
