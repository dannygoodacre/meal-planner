using MealPlanner.Data;
using MealPlanner.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.IntegrationTests;

public sealed class SmokeTests(PostgresFixture postgres) : IntegrationTestBase(postgres)
{
    [Fact]
    public async Task Migrations_AreApplied()
    {
        List<string> pending = await Host.InScopeAsync(async sp =>
            (await sp.GetRequiredService<ApplicationContext>().Database
                .GetPendingMigrationsAsync(TestContext.Current.CancellationToken)).ToList());

        Assert.Empty(pending);
    }

    [Fact]
    public async Task PgTrgmExtension_Exists()
    {
        bool exists = await Host.InScopeAsync(async sp =>
            await sp.GetRequiredService<ApplicationContext>().Database
                .SqlQueryRaw<bool>("SELECT EXISTS (SELECT 1 FROM pg_extension WHERE extname = 'pg_trgm') AS \"Value\"")
                .SingleAsync(TestContext.Current.CancellationToken));

        Assert.True(exists);
    }

    [Fact]
    public async Task Interceptor_RecordsCommands()
    {
        await Host.SeedFoodAsync();

        Assert.NotEmpty(Host.DatabaseActivity.Commands);
    }
}
