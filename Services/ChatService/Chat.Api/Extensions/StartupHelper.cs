using Chat.Api.Configs;
using Chat.Api.Hubs;
using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Infrastructure.Persistence;
using Chat.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Serilog;
using StackExchange.Redis;

namespace Chat.Api.Extensions
{
    public static class StartupHelper
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddSignalR();
            services.AddOpenApi();
            services.AddSerilog((context, conf) =>
                conf.ReadFrom.Configuration(configuration));

            services.AddChatServices();

            services.Configure<ServicesConfig>(configuration.GetSection("ServicesConfig"));
            services.AddHttpClient<ICatalogService, CatalogService>("CatalogApi", (provider, client) =>
            {
                var config = provider.GetRequiredService<IOptions<ServicesConfig>>().Value;
                client.BaseAddress = new Uri(config.CatalogApi!.BaseUrl);
            });


            var dbConnection = configuration.GetConnectionString("Postgres");
            services.AddDbContext<ChatDbContext>(options =>
                options.UseNpgsql(dbConnection));

            var redisConnection = configuration.GetConnectionString("Redis");
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnection));

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        public static async Task ApplyMigrationsAndSeedAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

            await context.Database.MigrateAsync();

            if (app.Environment.IsDevelopment())
            {
                await ChatDbSeeder.SeedAsync(context);
            }
        }

        public static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options
                        .WithTitle("ChatService API")
                        .WithTheme(ScalarTheme.BluePlanet);
                });
            }

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/hubs/chat");
            app.MapControllers();
        }
    }
}
