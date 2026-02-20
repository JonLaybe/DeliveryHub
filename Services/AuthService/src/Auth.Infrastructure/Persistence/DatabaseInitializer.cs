using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.Infrastructure.Persistence;

public interface IDatabaseInitializer
{
    Task InitializeAsync(IHostEnvironment env, CancellationToken ct = default);
}

public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly AuthDbContext _db;
    private readonly IConfiguration _cfg;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(AuthDbContext db, IConfiguration cfg, ILogger<DatabaseInitializer> logger)
    {
        _db = db;
        _cfg = cfg;
        _logger = logger;
    }

    public async Task InitializeAsync(IHostEnvironment env, CancellationToken ct = default)
    {
        var applyMigrationsRaw = _cfg["APPLY_MIGRATIONS"];
        var applyMigrations = string.IsNullOrWhiteSpace(applyMigrationsRaw)
            ? false
            : bool.TryParse(applyMigrationsRaw, out var v) && v;

        if (!applyMigrations && !env.IsDevelopment())
        {
            _logger.LogInformation("Skipping migrations (APPLY_MIGRATIONS=false and not Development).");
            return;
        }

        _logger.LogInformation("Applying migrations...");
        await _db.Database.MigrateAsync(ct);

        _logger.LogInformation("Seeding roles...");
        await SeedRolesAsync(ct);
    }

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        if (await _db.Roles.AnyAsync(ct))
            return;

        _db.Roles.AddRange(
            new Role { Name = "Admin", Description = "System administrator" },
            new Role { Name = "Customer", Description = "Customer role" }
        );

        await _db.SaveChangesAsync(ct);
    }
}
