using DannyGoodacre.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace MealPlanner.Application;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddCqrsHandlers(typeof(ServiceCollectionExtensions).Assembly);

            return services;
        }
    }
}
