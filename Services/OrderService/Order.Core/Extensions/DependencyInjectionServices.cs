using Microsoft.Extensions.DependencyInjection;
using OrderService.Core.Services.Interfaces.Orders;
using OrderService.Core.Services.Orders;

namespace OrderService.Core.Extensions
{
    public static class DependencyInjectionServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            _ = services.AddTransient<IOrderService, OrdersService>();

            return services;
        }
    }
}
