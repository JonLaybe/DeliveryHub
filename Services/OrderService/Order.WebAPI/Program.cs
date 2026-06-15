using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Persistence;
using OrderService.WebAPI.Extensions;
using Shared.RabbitMq.Interfaces;

namespace OrderService.WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddSwaggerGen(options =>
            {
                var basePath = AppContext.BaseDirectory;
                var xmlFiles = Directory.GetFiles(basePath, "*.xml");

                foreach (var xmlPath in xmlFiles)
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });


            builder.Services.AddControllers();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddHttpClient();
            builder.Services.AddAuth(builder.Configuration);
            builder.Services.RegisterDependencies(builder.Configuration);

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

            var rabbitClient = app.Services.GetRequiredService<IClientRabbitMq>();
            await rabbitClient.Connection();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    var context = services.GetRequiredService<ApplicationDbContext>();
                    if ((await context.Database.GetPendingMigrationsAsync()).Any())
                    {
                        await context.Database.MigrateAsync();
                    }
                }
            }

            app.UseCors("AllowAll");

            // Configure the HTTP request pipeline.

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
