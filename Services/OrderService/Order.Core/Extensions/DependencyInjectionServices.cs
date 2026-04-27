using Microsoft.Extensions.DependencyInjection;
using OrderService.Core.Services.Interfaces.Orders;
using OrderService.Core.Services.Interfaces.Products;
using OrderService.Core.Services.Interfaces.Users;
using OrderService.Core.Services.Orders;
using OrderService.Core.Services.Products;
using OrderService.Core.Services.Users;

namespace OrderService.Core.Extensions
{
    public static class DependencyInjectionServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            _ = services.AddScoped<IOrderService, OrdersService>()
                .AddScoped<IProductService, ProductService>()
                .AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
