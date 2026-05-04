using Chat.Api.Hubs;
using Chat.Infrastructure.Persistence;
using Chat.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
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

            services.AddChatServices();

            var dbConnection = configuration.GetConnectionString("Postgres");
            services.AddDbContext<ChatDbContext>(options =>
                options.UseNpgsql(dbConnection));

            var redisConnection = configuration.GetConnectionString("Redis");
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisConnection));
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

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/chat");
            app.MapControllers();
        }
    }
}
