using Microsoft.Extensions.DependencyInjection;
using OrderService.Core.Repositories.Interfaces.Orders;
using OrderService.Core.Repositories.Orders;

namespace OrderService.Core.Extensions
{
    public static class DependencyInjectionRepositories
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            _ = services.AddTransient<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
