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

    public void Remove(MealPlan mealPlan)
        => context.MealPlans.Remove(mealPlan);

    public Task<MealPlan?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => context.MealPlans
            .Include(x => x.Meals)
            .ThenInclude(x => x.Ingredients)
            .ThenInclude(x => x.Food)
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        => context.MealPlans
            .CountAsync(cancellationToken);

    public Task<List<MealPlan>> GetPaginatedAsync(int page, int limit, CancellationToken cancellationToken = default)
        => context.MealPlans
            .OrderBy(x => x.Name)
            .Include(x => x.Meals)
            .ThenInclude(x => x.Ingredients)
            .ThenInclude(x => x.Food)
            .Skip(page * limit)
            .Take(limit)
            .ToListAsync(cancellationToken);

    public Task<MealPlan?> GetWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
        => context.MealPlans
            .AsTracking()
            .Include(x => x.Meals)
            .ThenInclude(x => x.Ingredients)
            .ThenInclude(x => x.Food)
            .AsSplitQuery()
            .SingleOrDefaultAsync(x => x.PublicId == id, cancellationToken);

    public Task<List<MealPlan>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        string normalizedTerm = searchTerm.ToNormalizedString();

        return context.MealPlans
            .Where(x =>
                x.NormalizedName.Contains(normalizedTerm)
                || EF.Functions.TrigramsSimilarity(x.NormalizedName, normalizedTerm) > 0.2)
            .OrderByDescending(x =>
                x.NormalizedName.StartsWith(normalizedTerm)
                    ? 1.0
                    : EF.Functions.TrigramsSimilarity(x.NormalizedName, normalizedTerm))
            .Take(10)
            .Include(x => x.Meals)
            .ThenInclude(x => x.Ingredients)
            .ThenInclude(x => x.Food)
            .ToListAsync(cancellationToken);
    }
}
