using Auth.Infrastructure.Persistence;
using Auth.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/var/dpkeys"))
    .SetApplicationName("AuthService"); ;

builder.Services.AddAuthInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => Results.Ok("AuthService is running"));
app.MapHealthChecks("/health");

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await initializer.InitializeAsync(app.Environment);
}

app.Run();


