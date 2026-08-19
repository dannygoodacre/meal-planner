using MealPlanner.Application;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Data.Repositories;

internal sealed class FoodRepository(ApplicationContext context) : IFoodRepository
{
    public Food Add(Food food)
        => context.Foods.Add(food).Entity;

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        => context.Foods
            .AnyAsync(x => x.NormalizedName == name.ToNormalizedString(), cancellationToken);

    public void Remove(Food food)
        => context.Remove(food);

    public Task<Food?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Foods
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<int> GetCountAsync(CancellationToken cancellationToken)
        => context.Foods
            .CountAsync(cancellationToken);

    public Task<Dictionary<Guid, int>> GetIdMappingAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        => context.Foods
            .Where(x => ids.Contains(x.PublicId))
            .ToDictionaryAsync(x => x.PublicId, x => x.Id, cancellationToken);

    public Task<List<Food>> GetPaginatedAsync(int page, int limit, CancellationToken cancellationToken = default)
        => context.Foods
            .OrderBy(x => x.Name)
            .Skip(page * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public Task<Food?> GetWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Foods
            .AsTracking()
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<List<Food>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        string normalizedTerm = searchTerm.ToNormalizedString();

        return context.Foods
            .Where(x =>
                x.NormalizedName.Contains(normalizedTerm)
                || EF.Functions.TrigramsSimilarity(x.NormalizedName, normalizedTerm) > 0.2)
            .OrderByDescending(x =>
                x.NormalizedName.StartsWith(normalizedTerm)
                    ? 1.0
                    : EF.Functions.TrigramsSimilarity(x.NormalizedName, normalizedTerm))
            .Take(10)
            .ToListAsync(cancellationToken);
    }
}
