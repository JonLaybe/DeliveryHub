using DiscountService.Api.Infrastructure;
using DiscountService.Core.Repositories;
using DiscountService.Core.Services;
using DiscountService.Core.Services.Abstractions;
using DiscountService.Data;
using DiscountService.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Api
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();

            // Регистрация DbContext (раскомментируйте и настройте)
            var connection = builder.Configuration.GetConnectionString("DiscountDb");
            builder.Services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(connection));

            // Регистрация сервисов (раскомментируйте и реализуйте)
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddScoped<IDiscountProcessor, DiscountProcessor>();


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

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
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
                db.Database.Migrate(); // Применяет миграции при запуске
            }

            app.Run();
        }
    }
}

