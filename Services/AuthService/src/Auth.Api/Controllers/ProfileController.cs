using Auth.Api.Contracts.Profile;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
public sealed class ProfileController : ControllerBase
{
    private readonly UserManager<User> _userManager;

    public ProfileController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(user);

        var response = new UserProfileResponse(
            Id: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            PhotoUrl: user.PhotoUrl,
            BirthDate: user.BirthDate,
            PhoneNumber: user.PhoneNumber,
            Country: user.Country,
            City: user.City,
            Roles: roles.ToList()
        );

        return Ok(response);
    }
}