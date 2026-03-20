using Auth.Application.UseCases.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthApplication(this IServiceCollection services)
    {
        // UseCases
        services.AddScoped<CreateUser>();
        services.AddScoped<AssignRoleToUser>();

        return services;
    }
}