using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddCommandHandlers([typeof(ServiceCollectionExtensions).Assembly]);

            services.AddQueryHandlers([typeof(ServiceCollectionExtensions).Assembly]);

            return services;
        }
    }
}
