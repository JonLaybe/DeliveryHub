using Microsoft.Extensions.DependencyInjection;
using OrderService.Core.MappingProfile;

namespace OrderService.Core.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddAutoMapper((config) =>
            {
                config.AddProfiles([new OrderProfile()]);
            });

            _ = services.AddRepositories();

            return services;
        }
    }
}
