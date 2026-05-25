using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Net.Sockets;
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

        const int maxAttempts = 15;
        var delay = TimeSpan.FromSeconds(1);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                _logger.LogInformation("Applying migrations... (attempt {Attempt}/{Max})", attempt, maxAttempts);
                await _db.Database.MigrateAsync(ct);

                _logger.LogInformation("Migrations applied (or already up to date).");
                break;
            }
            catch (Exception ex) when (IsDbNotReady(ex) && attempt < maxAttempts)
            {
                _logger.LogWarning(ex,
                    "Database is not ready yet (attempt {Attempt}/{Max}). Waiting {Delay}...",
                    attempt, maxAttempts, delay);

                await Task.Delay(delay, ct);

                var nextSeconds = Math.Min(delay.TotalSeconds * 1.5, 10);
                delay = TimeSpan.FromSeconds(nextSeconds);
            }
        }

        _logger.LogInformation("Seeding roles...");
        await SeedRolesAsync(ct);

        _logger.LogInformation("Seeding service clients...");
        await SeedServiceClientsAsync(ct);
    }

    private static bool IsDbNotReady(Exception ex)
    {
        if (ex is PostgresException pg && pg.SqlState == "57P03")
            return true;

        if (ex is NpgsqlException npg)
        {
            if (npg.InnerException is SocketException se &&
                (se.SocketErrorCode == SocketError.ConnectionRefused
                 || se.SocketErrorCode == SocketError.TimedOut
                 || se.SocketErrorCode == SocketError.HostNotFound))
                return true;

            if (npg.InnerException is TimeoutException)
                return true;
        }

        return ex.InnerException is not null && IsDbNotReady(ex.InnerException);
    }

    private async Task SeedRolesAsync(CancellationToken ct)
    {
        if (await _db.Roles.AnyAsync(ct))
            return;

        _db.Roles.AddRange(
            new Role { Id = Guid.NewGuid(), Name = "Admin", NormalizedName = "ADMIN", Description = "System administrator" },
            new Role { Id = Guid.NewGuid(), Name = "Customer", NormalizedName = "CUSTOMER", Description = "Customer role" }
        );

        await _db.SaveChangesAsync(ct);
    }

    private async Task SeedServiceClientsAsync(CancellationToken ct)
    {
        var clients = _cfg.GetSection("ServiceClients").Get<List<ServiceClientSeedOptions>>();

        if (clients is null || clients.Count == 0)
        {
            _logger.LogInformation("No service clients configured for seed.");
            return;
        }

        foreach (var item in clients)
        {
            var exists = await _db.ServiceClients
                .AnyAsync(x => x.ClientId == item.ClientId, ct);

            if (exists)
                continue;

            _db.ServiceClients.Add(new ServiceClientEntity
            {
                Id = Guid.NewGuid(),
                ClientId = item.ClientId,
                SecretHash = Sha256Hex(item.ClientSecret),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        await _db.SaveChangesAsync(ct);
    }

    private sealed class ServiceClientSeedOptions
    {
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }
}
