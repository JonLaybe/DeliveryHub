using Auth.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.IntegrationTests;

public sealed class AuthApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IDatabaseInitializer>();
            services.AddScoped<IDatabaseInitializer, NoopDatabaseInitializer>();

            services.RemoveAll<AuthDbContext>();
            services.RemoveAll<DbContextOptions<AuthDbContext>>();

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseInMemoryDatabase($"auth-integration-tests-{Guid.NewGuid()}");
            });
        });
    }

    private sealed class NoopDatabaseInitializer : IDatabaseInitializer
    {
        public Task InitializeAsync(IHostEnvironment env, CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}