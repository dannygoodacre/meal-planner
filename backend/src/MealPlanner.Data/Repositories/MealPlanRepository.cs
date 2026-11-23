using MealPlanner.Application;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Data.Repositories;

internal sealed class MealPlanRepository(ApplicationContext context) : IMealPlanRepository
{
    public MealPlan Add(MealPlan mealPlan)
        => context.MealPlans.Add(mealPlan).Entity;

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        => context.MealPlans
            .AnyAsync(x => x.NormalizedName == name.ToNormalizedString(), cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, Guid excludeId, CancellationToken cancellationToken = default)
        => context.MealPlans
            .AnyAsync(x => x.NormalizedName == name.ToNormalizedString() && x.PublicId != excludeId, cancellationToken);

    public void Remove(MealPlan mealPlan)
        => context.MealPlans.Remove(mealPlan);

    public Task<MealPlan?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Query()
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<MealPlan?> GetWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
        => Query(tracking: true)
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<List<MealPlan>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var normalizedTerm = searchTerm.ToNormalizedString();

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

    private IQueryable<MealPlan> Query(bool tracking = false)
        => (tracking ? context.MealPlans.AsTracking() : context.MealPlans)
            .Include(x => x.Meals)
            .ThenInclude(x => x.Ingredients)
            .ThenInclude(x => x.Food);
}
