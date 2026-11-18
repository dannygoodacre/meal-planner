using DannyGoodacre.Cqrs.EntityFrameworkCore;
using MealPlanner.Application.Abstractions.Repositories;
using MealPlanner.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.Data;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddData(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationContext>(options =>
            {
                // Single query: the includes are a nested chain (no sibling collections), so there is no cartesian explosion to split.
                options.UseNpgsql(connectionString, npgsql => npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery));

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddEntityFrameworkUnits<ApplicationContext>();

            services.AddScoped<IFoodRepository, FoodRepository>();

            services.AddScoped<IMealRepository, MealRepository>();

            services.AddScoped<IMealPlanRepository, MealPlanRepository>();

            return services;
        }
    }
}
