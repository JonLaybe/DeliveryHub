using Auth.Api.Controllers;
using Auth.Api.Validation;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Persistence.Entities;
using Auth.Infrastructure.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.UnitTests.Controllers;

public sealed class ServiceTokenControllerTests
{
    private readonly IValidator<ServiceTokenController.ServiceTokenRequest> _validator =
        new ServiceTokenRequestValidator();

    [Fact]
    public async Task Issue_Should_Return_BadRequest_When_Request_Is_Null()
    {
        await using var db = CreateDbContext();
        var tokenService = new FakeJwtTokenService();
        var controller = CreateController(db, tokenService);

        var result = await controller.Issue(null!, _validator, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Issue_Should_Return_BadRequest_When_Request_Is_Invalid()
    {
        await using var db = CreateDbContext();
        var tokenService = new FakeJwtTokenService();
        var controller = CreateController(db, tokenService);

        var request = new ServiceTokenController.ServiceTokenRequest(
            "",
            "",
            "discount.read"
        );

        var result = await controller.Issue(request, _validator, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Issue_Should_Return_Unauthorized_When_Client_Not_Found()
    {
        await using var db = CreateDbContext();
        var tokenService = new FakeJwtTokenService();
        var controller = CreateController(db, tokenService);

        var request = new ServiceTokenController.ServiceTokenRequest(
            "unknown-service",
            "secret",
            "discount.read"
        );

        var result = await controller.Issue(request, _validator, CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Issue_Should_Return_Unauthorized_When_Client_Is_Inactive()
    {
        await using var db = CreateDbContext();

        db.ServiceClients.Add(new ServiceClientEntity
        {
            Id = Guid.NewGuid(),
            ClientId = "order-service",
            SecretHash = Sha256Hex("secret"),
            IsActive = false,
            CreatedAt = DateTimeOffset.UtcNow,
            AllowedScopes = "service discount.read"
        });

        await db.SaveChangesAsync();

        var tokenService = new FakeJwtTokenService();
        var controller = CreateController(db, tokenService);

        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "secret",
            "discount.read"
        );

        var result = await controller.Issue(request, _validator, CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Issue_Should_Return_Unauthorized_When_ClientSecret_Is_Invalid()
    {
        await using var db = CreateDbContext();

        db.ServiceClients.Add(new ServiceClientEntity
        {
            Id = Guid.NewGuid(),
            ClientId = "order-service",
            SecretHash = Sha256Hex("correct-secret"),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            AllowedScopes = "service discount.read"
        });

        await db.SaveChangesAsync();

        var tokenService = new FakeJwtTokenService();
        var controller = CreateController(db, tokenService);

        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "wrong-secret",
            "discount.read"
        );

        var result = await controller.Issue(request, _validator, CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Issue_Should_Return_Forbidden_When_Scope_Is_Not_Allowed()
    {
        await using var db = CreateDbContext();

        db.ServiceClients.Add(new ServiceClientEntity
        {
            Id = Guid.NewGuid(),
            ClientId = "order-service",
            SecretHash = Sha256Hex("secret"),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            AllowedScopes = "service discount.read"
        });

        await db.SaveChangesAsync();

        var tokenService = new FakeJwtTokenService();
        var controller = CreateController(db, tokenService);

        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "secret",
            "profile.write"
        );

        var result = await controller.Issue(request, _validator, CancellationToken.None);

        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, statusCodeResult.StatusCode);
    }

    [Fact]
    public async Task Issue_Should_Return_Ok_When_Request_Is_Valid()
    {
        await using var db = CreateDbContext();

        db.ServiceClients.Add(new ServiceClientEntity
        {
            Id = Guid.NewGuid(),
            ClientId = "order-service",
            SecretHash = Sha256Hex("secret"),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            AllowedScopes = "service discount.read profile.read"
        });

        await db.SaveChangesAsync();

        var tokenService = new FakeJwtTokenService();
        var controller = CreateController(db, tokenService);

        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "secret",
            "discount.read profile.read"
        );

        var result = await controller.Issue(request, _validator, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);

        Assert.Equal("fake-access-token", GetPropertyValue<string>(okResult.Value!, "access_token"));
        Assert.Equal("Bearer", GetPropertyValue<string>(okResult.Value!, "token_type"));
        Assert.Equal(900, GetPropertyValue<int>(okResult.Value!, "expires_in"));

        Assert.Contains(tokenService.Claims, claim => claim.Type == "sub" && claim.Value == "order-service");
        Assert.Contains(tokenService.Claims, claim => claim.Type == "typ" && claim.Value == "service");
        Assert.Contains(tokenService.Claims, claim => claim.Type == "scope" && claim.Value == "discount.read profile.read");
    }

    [Fact]
    public async Task Issue_Should_Use_Service_Scope_When_Request_Scope_Is_Empty()
    {
        await using var db = CreateDbContext();

        db.ServiceClients.Add(new ServiceClientEntity
        {
            Id = Guid.NewGuid(),
            ClientId = "order-service",
            SecretHash = Sha256Hex("secret"),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            AllowedScopes = "service discount.read"
        });

        await db.SaveChangesAsync();

        var tokenService = new FakeJwtTokenService();
        var controller = CreateController(db, tokenService);

        var request = new ServiceTokenController.ServiceTokenRequest(
            "order-service",
            "secret",
            null
        );

        var result = await controller.Issue(request, _validator, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result);
        Assert.Contains(tokenService.Claims, claim => claim.Type == "scope" && claim.Value == "service");
    }

    private static AuthDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AuthDbContext(options);
    }

    private static ServiceTokenController CreateController(
        AuthDbContext db,
        IJwtTokenService tokenService)
    {
        var jwtOptions = new JwtOptions
        {
            AccessTokenMinutes = 15
        };

        return new ServiceTokenController(db, tokenService, jwtOptions);
    }

    private static string Sha256Hex(string value)
    {
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(value))
        );
    }

    private static T GetPropertyValue<T>(object value, string propertyName)
    {
        var property = value.GetType().GetProperty(propertyName);

        Assert.NotNull(property);

        var propertyValue = property.GetValue(value);

        Assert.NotNull(propertyValue);

        return Assert.IsType<T>(propertyValue);
    }

    private sealed class FakeJwtTokenService : IJwtTokenService
    {
        public IReadOnlyCollection<Claim> Claims { get; private set; } = Array.Empty<Claim>();

        public string CreateAccessToken(IEnumerable<Claim> claims, DateTimeOffset nowUtc)
        {
            Claims = claims.ToArray();
            return "fake-access-token";
        }
    }
}