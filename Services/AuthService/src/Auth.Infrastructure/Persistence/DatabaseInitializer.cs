using Auth.Domain.Entities;
using Auth.Infrastructure.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
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

    private static readonly Guid AdminUserId =
        Guid.Parse("10000000-0000-0000-0000-000000000000");

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
        _logger.LogInformation(
            "Database initialization started. Environment: {EnvironmentName}",
            env.EnvironmentName);

        try
        {
            var applyMigrationsRaw = _cfg["APPLY_MIGRATIONS"];
            var applyMigrations = string.IsNullOrWhiteSpace(applyMigrationsRaw)
                ? false
                : bool.TryParse(applyMigrationsRaw, out var v) && v;

            _logger.LogInformation(
                "Database initialization settings loaded. APPLY_MIGRATIONS: {ApplyMigrations}",
                applyMigrations);

            if (!applyMigrations && !env.IsDevelopment())
            {
                _logger.LogInformation(
                    "Skipping migrations and seed data. APPLY_MIGRATIONS=false and environment is not Development.");

                return;
            }

            const int maxAttempts = 15;
            var delay = TimeSpan.FromSeconds(1);

            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    _logger.LogInformation(
                        "Applying migrations. Attempt {Attempt}/{MaxAttempts}",
                        attempt,
                        maxAttempts);

                    await _db.Database.MigrateAsync(ct);

                    _logger.LogInformation("Migrations applied successfully or database is already up to date.");
                    break;
                }
                catch (Exception ex) when (IsDbNotReady(ex) && attempt < maxAttempts)
                {
                    _logger.LogWarning(
                        ex,
                        "Database is not ready yet. Attempt {Attempt}/{MaxAttempts}. Waiting {DelaySeconds} seconds before retry.",
                        attempt,
                        maxAttempts,
                        delay.TotalSeconds);

                    await Task.Delay(delay, ct);

                    var nextSeconds = Math.Min(delay.TotalSeconds * 1.5, 10);
                    delay = TimeSpan.FromSeconds(nextSeconds);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to apply database migrations. Attempt {Attempt}/{MaxAttempts}",
                        attempt,
                        maxAttempts);

                    throw;
                }
            }

            _logger.LogInformation("Seeding roles started.");
            await SeedRolesAsync(ct);
            _logger.LogInformation("Seeding roles completed.");

            _logger.LogInformation("Seeding admin user started.");
            await SeedAdminUserAsync(ct);
            _logger.LogInformation("Seeding admin user completed.");

            _logger.LogInformation("Seeding default sellers started.");
            await SeedDefaultSellersAsync(ct);
            _logger.LogInformation("Seeding default sellers completed.");

            _logger.LogInformation("Seeding service clients started.");
            await SeedServiceClientsAsync(ct);
            _logger.LogInformation("Seeding service clients completed.");

            _logger.LogInformation("Database initialization completed successfully.");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Database initialization was cancelled.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed.");
            throw;
        }
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

            if (exists)
            {
                _logger.LogInformation(
                    "Role already exists. Role: {RoleName}",
                    role.Name);

                continue;
            }

            _db.Roles.Add(role);

            _logger.LogInformation(
                "Role added to seed. Role: {RoleName}",
                role.Name);
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

        _logger.LogInformation(
            "Service clients seed configuration loaded. Count: {Count}",
            clients.Count);

        foreach (var item in clients)
        {
            if (string.IsNullOrWhiteSpace(item.ClientId))
            {
                _logger.LogError("Service client seed failed. ClientId is empty.");

                throw new InvalidOperationException("Service client ClientId is required.");//TODO:убрать, когда перестанет валиться
            }

            var exists = await _db.ServiceClients
                .AnyAsync(x => x.ClientId == item.ClientId, ct);

            if (exists)
            {
                _logger.LogInformation(
                    "Service client already exists. ClientId: {ClientId}",
                    item.ClientId);

                continue;
            }

            if (string.IsNullOrWhiteSpace(item.ClientSecret))
            {
                _logger.LogError(
                    "Service client seed failed. ClientSecret is empty for ClientId: {ClientId}",
                    item.ClientId);

                throw new InvalidOperationException($"ClientSecret is required for service client '{item.ClientId}'.");//TODO:убрать, когда перестанет валиться
            }

            if (string.IsNullOrWhiteSpace(item.AllowedScopes))
            {
                _logger.LogError(
                    "Service client seed failed. AllowedScopes is empty for ClientId: {ClientId}",
                    item.ClientId);

                throw new InvalidOperationException($"AllowedScopes is required for service client '{item.ClientId}'.");//TODO:убрать, когда перестанет валиться
            }

            var normalizedScopes = NormalizeScopes(item.AllowedScopes);

            _db.ServiceClients.Add(new ServiceClientEntity
            {
                Id = Guid.NewGuid(),
                ClientId = item.ClientId,
                SecretHash = Sha256Hex(item.ClientSecret),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                AllowedScopes = normalizedScopes
            });

            _logger.LogInformation(
                "Service client added to seed. ClientId: {ClientId}, AllowedScopes: {AllowedScopes}",
                item.ClientId,
                normalizedScopes);
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

    private async Task SeedAdminUserAsync(CancellationToken ct)
    {
        const string adminEmail = "admin@deliveryhub.local";
        const string adminPassword = "Admin123!";

        var now = DateTimeOffset.UtcNow;

        var existingUser = await _userManager.FindByIdAsync(AdminUserId.ToString());

        if (existingUser is null)
        {
            existingUser = await _userManager.FindByEmailAsync(adminEmail);
        }

        if (existingUser is null)
        {
            _logger.LogInformation(
                "Admin user does not exist. Creating admin user with email {Email}",
                adminEmail);

            var user = new User
            {
                Id = AdminUserId,
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                Status = UserStatus.Active,
                CreatedAt = now,
                UpdatedAt = now,
                FirstName = "System",
                LastName = "Administrator"
            };

            var createResult = await _userManager.CreateAsync(user, adminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    createResult.Errors.Select(e => $"{e.Code}: {e.Description}"));

                _logger.LogError(
                    "Failed to create admin user {Email}. Errors: {Errors}",
                    adminEmail,
                    errors);

                throw new InvalidOperationException(
                    $"Failed to create admin user '{adminEmail}'. Errors: {errors}");
            }

            _logger.LogInformation(
                "Admin user created successfully. UserId: {UserId}, Email: {Email}",
                user.Id,
                user.Email);

            existingUser = user;
        }
        else
        {
            _logger.LogInformation(
                "Admin user already exists. UserId: {UserId}, Email: {Email}",
                existingUser.Id,
                existingUser.Email);
        }

        var isInAdminRole = await _userManager.IsInRoleAsync(existingUser, "Admin");

        if (!isInAdminRole)
        {
            _logger.LogInformation(
                "Adding Admin role to user {UserId}",
                existingUser.Id);

            var addRoleResult = await _userManager.AddToRoleAsync(existingUser, "Admin");

            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    addRoleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));

                _logger.LogError(
                    "Failed to add Admin role to user {UserId}. Errors: {Errors}",
                    existingUser.Id,
                    errors);

                throw new InvalidOperationException(
                    $"Failed to add role Admin to user '{adminEmail}'. Errors: {errors}");
            }

            _logger.LogInformation(
                "Admin role added successfully to user {UserId}",
                existingUser.Id);
        }
        else
        {
            _logger.LogInformation(
                "Admin user already has Admin role. UserId: {UserId}",
                existingUser.Id);
        }
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
                UserName = "seller.electronics@deliveryhub.local",
                FirstName = "Bill",
                LastName = "Gates",
                PhotoUrl = "https://i.pravatar.cc/300?u=seller.electronics@deliveryhub.local",
                BirthDate = new DateOnly(1990, 1, 10),
                PhoneNumber = "+70000000001",
                Country = "Russia",
                City = "Moscow"
            },
            new
            {
                Id = SellerClothesId,
                Email = "seller.clothes@deliveryhub.local",
                UserName = "seller.clothes@deliveryhub.local",
                FirstName = "Garavani",
                LastName = "Valentino",
                PhotoUrl = "https://i.pravatar.cc/300?u=seller.clothes@deliveryhub.local",
                BirthDate = new DateOnly(1991, 2, 15),
                PhoneNumber = "+70000000002",
                Country = "Russia",
                City = "Moscow"
            },
            new
            {
                Id = SellerFoodId,
                Email = "seller.food@deliveryhub.local",
                UserName = "seller.food@deliveryhub.local",
                FirstName = "Colonel",
                LastName = "Sanders",
                PhotoUrl = "https://i.pravatar.cc/300?u=seller.food@deliveryhub.local",
                BirthDate = new DateOnly(1992, 3, 20),
                PhoneNumber = "+70000000003",
                Country = "Russia",
                City = "Moscow"
            },
            new
            {
                Id = SellerBooksId,
                Email = "seller.books@deliveryhub.local",
                UserName = "seller.books@deliveryhub.local",
                FirstName = "Jeffrey",
                LastName = "Bezos",
                PhotoUrl = "https://i.pravatar.cc/300?u=seller.books@deliveryhub.local",
                BirthDate = new DateOnly(1993, 4, 25),
                PhoneNumber = "+70000000004",
                Country = "Russia",
                City = "Moscow"
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
                _logger.LogInformation(
                    "Default seller does not exist. Creating seller with email {Email}",
                    seller.Email);

                var user = new User
                {
                    Id = seller.Id,
                    Email = seller.Email,
                    UserName = seller.UserName,
                    FirstName = seller.FirstName,
                    LastName = seller.LastName,
                    PhotoUrl = seller.PhotoUrl,
                    BirthDate = seller.BirthDate,
                    PhoneNumber = seller.PhoneNumber,
                    Country = seller.Country,
                    City = seller.City,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
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

                    _logger.LogError(
                        "Failed to create default seller {Email}. Errors: {Errors}",
                        seller.Email,
                        errors);

                    throw new InvalidOperationException(
                        $"Failed to create default seller '{seller.Email}'. Errors: {errors}");
                }

                _logger.LogInformation(
                    "Default seller created successfully. UserId: {UserId}, Email: {Email}",
                    user.Id,
                    user.Email);

                existingUser = user;
            }
            else
            {
                _logger.LogInformation(
                    "Default seller already exists. UserId: {UserId}, Email: {Email}",
                    existingUser.Id,
                    existingUser.Email);
            }

            var profileWasChanged = false;

            if (existingUser.FirstName != seller.FirstName)
            {
                existingUser.FirstName = seller.FirstName;
                profileWasChanged = true;
            }

            if (existingUser.LastName != seller.LastName)
            {
                existingUser.LastName = seller.LastName;
                profileWasChanged = true;
            }

            if (existingUser.PhotoUrl != seller.PhotoUrl)
            {
                existingUser.PhotoUrl = seller.PhotoUrl;
                profileWasChanged = true;
            }

            if (existingUser.BirthDate != seller.BirthDate)
            {
                existingUser.BirthDate = seller.BirthDate;
                profileWasChanged = true;
            }

            if (existingUser.PhoneNumber != seller.PhoneNumber)
            {
                existingUser.PhoneNumber = seller.PhoneNumber;
                profileWasChanged = true;
            }

            if (existingUser.Country != seller.Country)
            {
                existingUser.Country = seller.Country;
                profileWasChanged = true;
            }

            if (existingUser.City != seller.City)
            {
                existingUser.City = seller.City;
                profileWasChanged = true;
            }

            if (profileWasChanged)
            {
                existingUser.UpdatedAt = now;

                _logger.LogInformation(
                    "Updating default seller profile. UserId: {UserId}, Email: {Email}",
                    existingUser.Id,
                    existingUser.Email);

                var updateResult = await _userManager.UpdateAsync(existingUser);

                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        updateResult.Errors.Select(e => $"{e.Code}: {e.Description}"));

                    _logger.LogError(
                        "Failed to update default seller profile {Email}. Errors: {Errors}",
                        seller.Email,
                        errors);

                    throw new InvalidOperationException(
                        $"Failed to update default seller profile '{seller.Email}'. Errors: {errors}");
                }

                _logger.LogInformation(
                    "Default seller profile updated successfully. UserId: {UserId}, Email: {Email}",
                    existingUser.Id,
                    existingUser.Email);
            }

            var isInSellerRole = await _userManager.IsInRoleAsync(existingUser, "Seller");

            if (!isInSellerRole)
            {
                _logger.LogInformation(
                    "Adding Seller role to user {UserId}",
                    existingUser.Id);

                var addRoleResult = await _userManager.AddToRoleAsync(existingUser, "Seller");

                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        addRoleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));

                    _logger.LogError(
                        "Failed to add Seller role to user {UserId}. Errors: {Errors}",
                        existingUser.Id,
                        errors);

                    throw new InvalidOperationException(
                        $"Failed to add role Seller to user '{seller.Email}'. Errors: {errors}");
                }

                _logger.LogInformation(
                    "Seller role added successfully to user {UserId}",
                    existingUser.Id);
            }
            else
            {
                _logger.LogInformation(
                    "Default seller already has Seller role. UserId: {UserId}",
                    existingUser.Id);
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