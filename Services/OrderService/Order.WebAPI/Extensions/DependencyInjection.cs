using OrderService.Core.Extensions;
using OrderService.Infrastructure.Extensions;

namespace OrderService.WebAPI.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCore();
            services.AddServices();
            services.AddInfrastructure(configuration);

            return services;
        }
    }
}
