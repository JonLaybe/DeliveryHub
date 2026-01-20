using OrderService.Core.Common.Interfaces;
using OrderService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var str = configuration.GetConnectionString("OrderDatabase");
            _ = services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("OrderDatabase"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });
            _ = services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            return services;
        }
    }
}
