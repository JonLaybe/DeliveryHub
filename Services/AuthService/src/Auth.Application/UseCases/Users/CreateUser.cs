using Auth.Application.Abstractions.Persistence;
using Auth.Application.Abstractions.Security;
using Auth.Domain.Entities;
using System;

namespace Auth.Application.UseCases.Users;

public sealed class CreateUser
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUserRoleRepository _userRoles;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;

    public CreateUser(
        IUserRepository users,
        IRoleRepository roles,
        IUserRoleRepository userRoles,
        IPasswordHasher hasher,
        IUnitOfWork uow)
    {
        _users = users;
        _roles = roles;
        _userRoles = userRoles;
        _hasher = hasher;
        _uow = uow;
    }

    public async Task<User> ExecuteAsync(string email, string password, CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();

        var existing = await _users.GetByEmailAsync(email, ct);
        if (existing is not null)
            throw new InvalidOperationException("EMAIL_ALREADY_EXISTS");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = _hasher.Hash(password),
            Status = UserStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _users.Add(user);

        // Customer - дефолтная роль
        var customerRole = await _roles.GetByNameAsync("Customer", ct);
        if (customerRole is not null)
        {
            _userRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = customerRole.Id,
                AssignedAt = DateTimeOffset.UtcNow
            });
        }

        await _uow.SaveChangesAsync(ct);
        return user;
    }
}