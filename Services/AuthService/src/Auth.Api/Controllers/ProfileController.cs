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

        var response = await CreateUserProfileResponseAsync(user);

        return Ok(response);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserProfileResponse>> UpdateMyProfile(
        [FromBody] UpdateMyProfileRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhotoUrl = request.PhotoUrl;
        user.BirthDate = request.BirthDate;
        user.PhoneNumber = request.PhoneNumber;
        user.Country = request.Country;
        user.City = request.City;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(ModelState);
        }

        var response = await CreateUserProfileResponseAsync(user);

        return Ok(response);
    }

    private async Task<UserProfileResponse> CreateUserProfileResponseAsync(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileResponse(
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
    }
}