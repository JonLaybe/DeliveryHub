using Auth.Application.Abstractions.Persistence;
using Auth.Domain.Entities;

namespace Auth.Application.UseCases.Users;

public sealed class AssignRoleToUser
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUserRoleRepository _userRoles;
    private readonly IUnitOfWork _uow;

    public AssignRoleToUser(IUserRepository users, IRoleRepository roles, IUserRoleRepository userRoles, IUnitOfWork uow)
    {
        _users = users;
        _roles = roles;
        _userRoles = userRoles;
        _uow = uow;
    }

    public async Task ExecuteAsync(Guid userId, string roleName, CancellationToken ct = default)
    {
        var user = await _users.GetByIdAsync(userId, ct) ?? throw new KeyNotFoundException("USER_NOT_FOUND");
        var role = await _roles.GetByNameAsync(roleName, ct) ?? throw new KeyNotFoundException("ROLE_NOT_FOUND");

        var exists = await _userRoles.ExistsAsync(user.Id, role.Id, ct);
        if (!exists)
        {
            _userRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id,
                AssignedAt = DateTimeOffset.UtcNow
            });

            await _uow.SaveChangesAsync(ct);
        }
    }
}