using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Abstractions.Repositories;

public interface IFoodRepository : IPublicEntityRepository<Food>
{
    /// <summary>
    /// Fetch a mapping of public IDs to internal database IDs.
    /// </summary>
    /// <param name="ids">The public IDs of the Food entities.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while performing the operation.</param>
    /// <returns>A <see cref="Dictionary{TKey, TValue}"/> with <see cref="Guid"/> keys and <see cref="int"/> values.</returns>
    Task<Dictionary<Guid, int>> GetIdMappingAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
