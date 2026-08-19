using MealPlanner.Application.Entities;

namespace MealPlanner.Application.Abstractions.Repositories;

public interface IMealRepository : IPublicEntityRepository<Meal>
{
    Task<List<Meal>> GetManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
