using MealPlanner.Application;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Data.Repositories;

internal sealed class MealRepository(ApplicationContext context) : IMealRepository
{
    public Meal Add(Meal meal)
        => context.Meals.Add(meal).Entity;

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        => context.Meals
            .AnyAsync(x => x.NormalizedName == name.ToNormalizedString(), cancellationToken);

    public void Remove(Meal meal)
        => context.Meals.Remove(meal);

    public Task<Meal?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Meals
            .Include(x => x.Ingredients)
            .ThenInclude(x => x.Food)
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        => context.Meals
            .CountAsync(cancellationToken);

    public Task<List<Meal>> GetManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        => context.Meals
            .Where(x => ids.Contains(x.PublicId))
            .Include(x => x.Ingredients)
            .ToListAsync(cancellationToken);

    public Task<List<Meal>> GetPaginatedAsync(int page, int limit, CancellationToken cancellationToken = default)
        => context.Meals
            .OrderBy(x => x.Name)
            .Skip(page * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public Task<Meal?> GetWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
        => context.Meals
            .AsTracking()
            .Include(x => x.Ingredients)
            .ThenInclude(x => x.Food)
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<List<Meal>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        string normalizedTerm = searchTerm.ToNormalizedString();

        return context.Meals
            .Include(x => x.Ingredients)
            .ThenInclude(x => x.Food)
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
