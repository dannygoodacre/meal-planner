using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Abstractions.Repositories;

public interface IFoodRepository : IPublicEntityRepository<Food>
{
    Task<bool> IsInUseAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> GetIdMappingAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
