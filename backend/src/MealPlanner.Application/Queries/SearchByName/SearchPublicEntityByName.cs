using DannyGoodacre.Cqrs;
using DannyGoodacre.Primitives;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.Application.Models;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Application.Queries;

internal sealed record SearchPublicEntityByNameQuery : IQuery
{
    public required string SearchTerm { get; init; }
}

internal abstract partial class SearchPublicEntityByNameHandler<TEntity, TResponse>(ILogger logger, IPublicEntityRepository<TEntity> repository)
    : QueryHandler<SearchPublicEntityByNameQuery, SearchResponse<TResponse>>(logger) where TEntity : PublicEntity
{
    protected override string QueryName => $"Search {typeof(TEntity).Name} By Name";

    protected override void Validate(ValidationState state, SearchPublicEntityByNameQuery query)
    {
        state.IsMinimumLength(query.SearchTerm, 3, nameof(query.SearchTerm));
    }

    protected async override Task<Result<SearchResponse<TResponse>>> InternalExecuteAsync(SearchPublicEntityByNameQuery query, CancellationToken cancellationToken = default)
    {
        LogQueryStarted(Logger, query.SearchTerm, query.SearchTerm);

        List<TEntity> matchingEntities = await repository.SearchByNameAsync(query.SearchTerm, cancellationToken);

        return Result.Success(new SearchResponse<TResponse>
        {
            Query = query.SearchTerm,
            Count = matchingEntities.Count,
            Items = matchingEntities.Select(ToResponse).ToList()
        });
    }

    public Task<Result<SearchResponse<TResponse>>> ExecuteAsync(string searchTerm, CancellationToken cancellationToken = default)
        => ExecuteAsync(new SearchPublicEntityByNameQuery
        {
            SearchTerm = searchTerm
        }, cancellationToken);

    protected abstract TResponse ToResponse(TEntity entity);

    [LoggerMessage(LogLevel.Information, "Query '{Query}' started for Search Term '{SearchTerm}'.")]
    private static partial void LogQueryStarted(ILogger logger, string query, string searchTerm);
}
