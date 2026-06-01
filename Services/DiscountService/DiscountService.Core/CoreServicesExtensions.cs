using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscountService.Core
{
    public static class CoreServicesExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            // MediatR requests registration        
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CoreServicesExtensions).GetTypeInfo().Assembly));

            return services;
        }

    }
}
