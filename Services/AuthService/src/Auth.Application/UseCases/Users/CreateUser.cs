using Auth.Application.Abstractions.Persistence;
using Auth.Application.Abstractions.Security;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Auth.Application.UseCases.Users;

public sealed class CreateUser
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public CreateUser(UserManager<User> userManager, RoleManager<Role> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<User> ExecuteAsync(string email, string password, CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();

        //проверка уникальности email
        var existing = await _userManager.FindByEmailAsync(email); 
        if (existing is not null)
            throw new InvalidOperationException("EMAIL_ALREADY_EXISTS");

        var now = DateTimeOffset.UtcNow;

        // создание пользователя (UserName обязателен для таблицы users)
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            Status = UserStatus.Active,
            CreatedAt = now,
            UpdatedAt = now
        };

        // создание пользователя с помощью Identity (внутри дополнительно кладёт PasswordHash через IPasswordHasher<User> (bcrypt))
        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            throw new InvalidOperationException("CREATE_USER_FAILED: " + string.Join("; ", createResult.Errors.Select(e => $"{e.Code}:{e.Description}")));

        // дефолтная роль Customer
        const string defaultRole = "Customer";
        if (!await _roleManager.RoleExistsAsync(defaultRole))
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = defaultRole,
                NormalizedName = defaultRole.ToUpperInvariant(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = now,
                Description = "Customer role"
            };

            var roleResult = await _roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
                throw new InvalidOperationException("CREATE_ROLE_FAILED: " + string.Join("; ", roleResult.Errors.Select(e => $"{e.Code}:{e.Description}")));
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, defaultRole);
        if (!addRoleResult.Succeeded)
            throw new InvalidOperationException("ADD_ROLE_FAILED: " + string.Join("; ", addRoleResult.Errors.Select(e => $"{e.Code}:{e.Description}")));

        return user;
    }
}