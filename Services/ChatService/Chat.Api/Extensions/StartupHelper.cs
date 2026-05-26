using Chat.Api.Configs;
using Chat.Api.Hubs;
using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Infrastructure.Persistence;
using Chat.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

namespace Chat.Api.Extensions
{
    public static class StartupHelper
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddSignalR();
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

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "ChatServiceCache";
            });

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

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ChatService API",
                    Version = "v1",
                    Description = "API для управления диалогами и сообщениями между покупателями и продавцами",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.EnableAnnotations();
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

                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatService API v1");
                    options.RoutePrefix = "swagger";
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
