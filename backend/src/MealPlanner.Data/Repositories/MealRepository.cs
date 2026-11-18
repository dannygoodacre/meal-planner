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

    public Task<bool> ExistsByNameAsync(string name, Guid excludeId, CancellationToken cancellationToken = default)
        => context.Meals
            .AnyAsync(x => x.NormalizedName == name.ToNormalizedString() && x.PublicId != excludeId, cancellationToken);

    public void Remove(Meal meal)
        => context.Meals.Remove(meal);

    public Task<Meal?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Query()
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<Meal?> GetWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
        => Query(tracking: true)
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<List<Meal>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        string normalizedTerm = searchTerm.ToNormalizedString();

        return Query()
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

    /// <summary>
    /// The full aggregate graph, shared by every read so the includes can't drift apart.
    /// </summary>
    private IQueryable<Meal> Query(bool tracking = false)
        => (tracking ? context.Meals.AsTracking() : context.Meals)
            .Include(x => x.Ingredients)
            .ThenInclude(x => x.Food);
}
