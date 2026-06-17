using DiscountService.Core.Repositories;
using DiscountService.Core.Services;
using DiscountService.Core.Services.Abstractions;
using DiscountService.Data;
using DiscountService.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using DiscountService.Core;
using DiscountService.Api.Infrastructure;
using MassTransit;
using DiscountService.Api.Consumers;

namespace DiscountService.Api
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            // Core
            builder.Services.AddCoreServices();
            builder.Services.AddHttpClient();
            builder.Services.AddAuth(builder.Configuration);

            // Регистрация DbContext (раскомментируйте и настройте)
            var connection = builder.Configuration.GetConnectionString("DiscountDb");
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(connection));

            // Регистрация сервисов (раскомментируйте и реализуйте)
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddScoped<IDiscountProcessor, DiscountProcessor>();

            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<OrderConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var host = builder.Configuration["RabbitMq:Host"];
                    cfg.Host(host, "/");

                    // Auto-configure endpoints for registered consumers
                    //cfg.ConfigureEndpoints(context);

                    cfg.ReceiveEndpoint("Product", e =>
                    {
                        e.UseRawJsonDeserializer(isDefault: true);

                        e.SetQueueArgument("x-dead-letter-exchange", "");
                        e.SetQueueArgument("x-dead-letter-routing-key", "ProductDeadLetter");
                        e.SetQueueArgument("x-dead-letter-strategy", "at-least-once");
                        e.SetQueueArgument("x-overflow", "reject-publish");
                        e.SetQueueArgument("x-queue-type", "quorum");

                        e.ConfigureConsumer<OrderConsumer>(context);
                    });
                });
            });
            // Swagger configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "DiscountService API",
                    Version = "v1",
                    Description = "API для управления скидками"
                });
            });

            builder.Services.AddSwaggerGenNewtonsoftSupport();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowAll");
            app.UseAuthorization();

            // Swagger middleware
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DiscountService v1");
                c.RoutePrefix = "swagger";
                c.DocumentTitle = "DiscountService API Docs";
            });

            app.UseReDoc(c =>
            {
                c.SpecUrl("/swagger/v1/swagger.json");
                c.RoutePrefix = "redoc";
                c.DocumentTitle = "DiscountService API Docs";
            });

            app.MapControllers();

            // Миграции базы данных
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                await db.Database.MigrateAsync(); // Применяет миграции при запуске

                await DatabaseInitializer.InitializeAsync(db);
            }

            app.Run();
        }
    }
}

