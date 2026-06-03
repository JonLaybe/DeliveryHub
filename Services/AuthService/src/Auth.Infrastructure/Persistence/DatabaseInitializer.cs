using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Auth.Infrastructure.Persistence.Entities;

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
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    private static readonly Guid AdminRoleId =
    Guid.Parse("00000000-0000-0000-0000-000000000001");

    private static readonly Guid CustomerRoleId =
        Guid.Parse("00000000-0000-0000-0000-000000000002");

    private static readonly Guid SellerRoleId =
        Guid.Parse("00000000-0000-0000-0000-000000000003");

    private static readonly Guid SellerElectronicsId =
        Guid.Parse("10000000-0000-0000-0000-000000000001");

    private static readonly Guid SellerClothesId =
        Guid.Parse("10000000-0000-0000-0000-000000000002");

    private static readonly Guid SellerFoodId =
        Guid.Parse("10000000-0000-0000-0000-000000000003");

    private static readonly Guid SellerBooksId =
        Guid.Parse("10000000-0000-0000-0000-000000000004");

    public DatabaseInitializer(
        AuthDbContext db,
        IConfiguration cfg,
        ILogger<DatabaseInitializer> logger,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _db = db;
        _cfg = cfg;
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
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

        _logger.LogInformation("Seeding default sellers...");
        await SeedDefaultSellersAsync(ct);

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
        var now = DateTimeOffset.UtcNow;

        var roles = new[]
        {
            new Role
            {
                Id = AdminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "System administrator",
                ConcurrencyStamp = AdminRoleId.ToString(),
                CreatedAt = now
            },
            new Role
            {
                Id = CustomerRoleId,
                Name = "Customer",
                NormalizedName = "CUSTOMER",
                Description = "Customer role",
                ConcurrencyStamp = CustomerRoleId.ToString(),
                CreatedAt = now
            },
            new Role
            {
                Id = SellerRoleId,
                Name = "Seller",
                NormalizedName = "SELLER",
                Description = "Seller role",
                ConcurrencyStamp = SellerRoleId.ToString(),
                CreatedAt = now
            }
        };

        foreach (var role in roles)
        {
            var exists = await _db.Roles
                .AnyAsync(x => x.NormalizedName == role.NormalizedName, ct);

            if (!exists)
                _db.Roles.Add(role);
        }

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

            if (string.IsNullOrWhiteSpace(item.ClientId))
                throw new InvalidOperationException("Service client ClientId is required.");//TODO:убрать, когда перестанет валиться

            if (string.IsNullOrWhiteSpace(item.ClientSecret))
                throw new InvalidOperationException($"ClientSecret is required for service client '{item.ClientId}'.");//TODO:убрать, когда перестанет валиться

            if (string.IsNullOrWhiteSpace(item.AllowedScopes))
                throw new InvalidOperationException($"AllowedScopes is required for service client '{item.ClientId}'.");//TODO:убрать, когда перестанет валиться

            _db.ServiceClients.Add(new ServiceClientEntity
            {
                Id = Guid.NewGuid(),
                ClientId = item.ClientId,
                SecretHash = Sha256Hex(item.ClientSecret),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                AllowedScopes = NormalizeScopes(item.AllowedScopes)
            });
        }

        await _db.SaveChangesAsync(ct);
    }

    private static string Sha256Hex(string value)
    {
        return Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(value)
            )
        );
    }

    private static string NormalizeScopes(string? scopes)
    {
        if (string.IsNullOrWhiteSpace(scopes))
            throw new InvalidOperationException("AllowedScopes is required for service client.");

        return string.Join(
            ' ',
            scopes
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
        );
    }

    private async Task SeedDefaultSellersAsync(CancellationToken ct)
    {
        const string defaultPassword = "Seller123!"; //для всех одинаковый

        var now = DateTimeOffset.UtcNow;

        var sellers = new[]
        {
            new
            {
                Id = SellerElectronicsId,
                Email = "seller.electronics@deliveryhub.local",
                UserName = "seller.electronics@deliveryhub.local"
            },
            new
            {
                Id = SellerClothesId,
                Email = "seller.clothes@deliveryhub.local",
                UserName = "seller.clothes@deliveryhub.local"
            },
            new
            {
                Id = SellerFoodId,
                Email = "seller.food@deliveryhub.local",
                UserName = "seller.food@deliveryhub.local"
            },
            new
            {
                Id = SellerBooksId,
                Email = "seller.books@deliveryhub.local",
                UserName = "seller.books@deliveryhub.local"
            }
        };

        foreach (var seller in sellers)
        {
            var existingUser = await _userManager.FindByIdAsync(seller.Id.ToString());

            if (existingUser is null)
            {
                existingUser = await _userManager.FindByEmailAsync(seller.Email);
            }

            if (existingUser is null)
            {
                var user = new User
                {
                    Id = seller.Id,
                    Email = seller.Email,
                    UserName = seller.UserName,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    Status = UserStatus.Active,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                var createResult = await _userManager.CreateAsync(user, defaultPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        createResult.Errors.Select(e => $"{e.Code}: {e.Description}"));

                    throw new InvalidOperationException(
                        $"Failed to create default seller '{seller.Email}'. Errors: {errors}");
                }

                existingUser = user;
            }

            var isInSellerRole = await _userManager.IsInRoleAsync(existingUser, "Seller");

            if (!isInSellerRole)
            {
                var addRoleResult = await _userManager.AddToRoleAsync(existingUser, "Seller");

                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        addRoleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));

                    throw new InvalidOperationException(
                        $"Failed to add role Seller to user '{seller.Email}'. Errors: {errors}");
                }
            }
        }
    }

    private sealed class ServiceClientSeedOptions
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? AllowedScopes { get; set; }
    }
}
