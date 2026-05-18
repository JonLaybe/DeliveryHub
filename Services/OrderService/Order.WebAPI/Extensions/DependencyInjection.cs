using OrderService.Core.Extensions;
using OrderService.Infrastructure.Extensions;
using Shared.Domain;
using Shared.RabbitMq;
using Shared.RabbitMq.Interfaces;

namespace OrderService.WebAPI.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitmqClientSettings = configuration.GetSection("RabbitMQ").Get<RabbitMqClientSettings>()
                ?? throw new InvalidOperationException("RabbitMQ settings are missing in appsettings.json");

            _ = services.AddSingleton(rabbitmqClientSettings)
                .AddSingleton<IClientRabbitMq, RabbitMqClient>();

            services.AddCore();
            services.AddServices();
            services.AddInfrastructure(configuration);

            return services;
        }
    }
}
