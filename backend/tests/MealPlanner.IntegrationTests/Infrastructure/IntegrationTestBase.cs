namespace MealPlanner.IntegrationTests.Infrastructure;

/// <summary>
/// Gives each test a clean database and a fresh host.
/// </summary>
public abstract class IntegrationTestBase(PostgresFixture postgres) : IAsyncLifetime
{
    internal TestHost Host { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await postgres.ResetAsync();

        Host = new TestHost(postgres.ConnectionString);

        Host.DatabaseActivity.Reset();
    }

    public async ValueTask DisposeAsync() => await Host.DisposeAsync();
}
