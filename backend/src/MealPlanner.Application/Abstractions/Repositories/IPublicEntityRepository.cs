using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Abstractions.Repositories;

public interface IPublicEntityRepository<TEntity> where TEntity : PublicEntity
{
    TEntity Add(TEntity entity);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    void Remove(TEntity entity);

    Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    Task<List<TEntity>> GetPaginatedAsync(int page, int limit, CancellationToken cancellationToken = default);

    Task<TEntity?> GetWithTrackingAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<TEntity>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
}
