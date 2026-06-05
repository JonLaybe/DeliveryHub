using Auth.Application.Abstractions.Persistence;
using Auth.Application.Abstractions.Security;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Persistence.Repositories;
using Auth.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IRoleRepository, EfRoleRepository>();
        services.AddScoped<IUserRoleRepository, EfUserRoleRepository>();
        services
            .AddIdentityCore<User>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Lockout.AllowedForNewUsers = true;
                o.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

        services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<User>, BcryptIdentityPasswordHasher>();
        
        var jwt = new JwtOptions
        {
            Issuer = configuration["Jwt:Issuer"] ?? "AuthService",
            AccessTokenMinutes = int.Parse(configuration["Jwt:AccessTokenMinutes"] ?? "15"),
            RefreshTokenDays = int.Parse(configuration["Jwt:RefreshTokenDays"] ?? "30"),
            KeyId = configuration["Jwt:KeyId"] ?? "auth-key-001",
            PrivateKeyPem = configuration["Jwt:PrivateKeyPem"],
            PublicKeyPem = configuration["Jwt:PublicKeyPem"],
            PrivateKeyPath = configuration["Jwt:PrivateKeyPath"],
            PublicKeyPath = configuration["Jwt:PublicKeyPath"]
        };

        services.AddSingleton(jwt);
        services.AddSingleton<IRsaKeyProvider, RsaKeyProvider>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,

                    ValidateAudience = false,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                    {
                        var keyProvider = services
                            .BuildServiceProvider()
                            .GetRequiredService<IRsaKeyProvider>();

                        return new[]
                        {
                    new RsaSecurityKey(keyProvider.Public)
                    {
                        KeyId = jwt.KeyId
                    }
                        };
                    },

                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,

                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization();

        return services;
    }
}