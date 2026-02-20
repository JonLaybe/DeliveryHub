using Auth.Api.Contracts.Users;
using Auth.Application.UseCases.Users;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Route("admin/users")]
public sealed class AdminUsersController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateUserResponse>> Create(
        [FromServices] CreateUser useCase,
        [FromBody] CreateUserRequest request,
        CancellationToken ct)
    {
        var user = await useCase.ExecuteAsync(request.Email, request.Password, ct);
        return Created($"/admin/users/{user.Id}", new CreateUserResponse(user.Id, user.Email));
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AssignRole(
        [FromServices] AssignRoleToUser useCase,
        [FromRoute] Guid id,
        [FromBody] AssignRoleRequest request,
        CancellationToken ct)
    {
        await useCase.ExecuteAsync(id, request.RoleName, ct);
        return NoContent();
    }
}