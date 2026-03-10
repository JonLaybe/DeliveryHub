using Chat.Application.Interfaces;
using Chat.Application.Services;
using Chat.Infrastructure.Persistence;
using Chat.Infrastructure.Repositories;
using Chat.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace Chat.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
            builder.Services.AddScoped<IConversationService, ConversationService>();

            var connection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ChatDbContext>(options => options.UseNpgsql(connection));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
                    await ChatDbSeeder.SeedAsync(context);
                }

                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options
                        .WithTitle("ChatService API")
                        .WithTheme(ScalarTheme.BluePlanet);
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
