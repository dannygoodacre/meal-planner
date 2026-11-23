using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Application.Entities;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.IntegrationTests.Repositories;

public abstract class RepositoryTestBase(PostgresFixture postgres) : IntegrationTestBase(postgres)
{
    public const string Food = "Food";
    public const string Meal = "Meal";
    public const string MealPlan = "MealPlan";

    public static TheoryData<string> Kinds => [Food, Meal, MealPlan];

    protected Task<Guid> SeedAsync(string kind, string name) => kind switch
    {
        Food => Host.SeedFoodAsync(name),
        Meal => Host.SeedMealAsync(name),
        _ => Host.SeedMealPlanAsync(name)
    };

    protected static IPublicEntityRepository<PublicEntity> Repository(IServiceProvider sp, string kind) => kind switch
    {
        Food => Adapt(sp.GetRequiredService<IFoodRepository>()),
        Meal => Adapt(sp.GetRequiredService<IMealRepository>()),
        _ => Adapt(sp.GetRequiredService<IMealPlanRepository>())
    };

    // The repositories are generic over the concrete entity, so wrap them to share one test body across the three kinds.
    private static Adapter<T> Adapt<T>(IPublicEntityRepository<T> repository) where T : PublicEntity => new(repository);

    private sealed class Adapter<T>(IPublicEntityRepository<T> inner) : IPublicEntityRepository<PublicEntity> where T : PublicEntity
    {
        public PublicEntity Add(PublicEntity entity) => inner.Add((T)entity);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
            => inner.ExistsByNameAsync(name, cancellationToken);

        public Task<bool> ExistsByNameAsync(string name, Guid excludeId, CancellationToken cancellationToken = default)
            => inner.ExistsByNameAsync(name, excludeId, cancellationToken);

        public void Remove(PublicEntity entity) => inner.Remove((T)entity);

        public async Task<PublicEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
            => await inner.GetAsync(id, cancellationToken);

        public async Task<PublicEntity?> GetWithTrackingAsync(Guid id, CancellationToken cancellationToken = default)
            => await inner.GetWithTrackingAsync(id, cancellationToken);

        public async Task<List<PublicEntity>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
            => [.. (await inner.SearchByNameAsync(searchTerm, cancellationToken)).Cast<PublicEntity>()];
    }
}
