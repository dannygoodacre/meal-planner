namespace MealPlanner.IntegrationTests.Infrastructure;

public abstract class IntegrationTestBase(PostgresFixture postgres) : IAsyncLifetime
{
    protected PostgresFixture Postgres => postgres;

    internal TestHost Host { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await postgres.ResetAsync();

        Host = new TestHost(postgres.ConnectionString);

        Host.DatabaseActivity.Reset();
    }

    public virtual async ValueTask DisposeAsync() => await Host.DisposeAsync();
}
