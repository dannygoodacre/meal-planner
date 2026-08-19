using DannyGoodacre.Cqrs;
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
                options.UseNpgsql(connectionString);

                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddScoped<IStateUnit>(x => x.GetRequiredService<ApplicationContext>());

            services.AddScoped<IFoodRepository, FoodRepository>();

            services.AddScoped<IMealRepository, MealRepository>();

            services.AddScoped<IMealPlanRepository, MealPlanRepository>();

            return services;
        }
    }
}
