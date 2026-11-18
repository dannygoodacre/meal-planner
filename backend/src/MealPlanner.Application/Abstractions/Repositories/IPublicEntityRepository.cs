using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Abstractions.Repositories;

public interface IPublicEntityRepository<TEntity> where TEntity : PublicEntity
{
    TEntity Add(TEntity entity);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Whether another entity (any except <paramref name="excludeId"/>) already has this name.
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, Guid excludeId, CancellationToken cancellationToken = default);

    void Remove(TEntity entity);

    Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TEntity?> GetWithTrackingAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<TEntity>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
}
