using Microsoft.Extensions.DependencyInjection;
using OrderService.Core.Repositories.Interfaces.Orders;
using OrderService.Core.Repositories.Interfaces.Products;
using OrderService.Core.Repositories.Orders;
using OrderService.Core.Repositories.Products;

namespace OrderService.Core.Extensions
{
    public static class DependencyInjectionRepositories
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            _ = services.AddScoped<IOrderRepository, OrderRepository>()
                .AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
