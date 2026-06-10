using Auth.Api.Contracts.Profile;
using Auth.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Auth.Api.Controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
public sealed class ProfileController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(
        UserManager<User> userManager,
        ILogger<ProfileController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileResponse>> GetMyProfile()
    {
        _logger.LogInformation("Profile request received for current user");

        try
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                _logger.LogWarning("Profile request failed. Current user was not found");

                return Unauthorized();
            }

            var response = await CreateUserProfileResponseAsync(user);

            _logger.LogInformation(
                "Profile successfully returned for user {UserId}",
                user.Id);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting current user profile");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserProfileResponse>> UpdateMyProfile(
        [FromBody] UpdateMyProfileRequest request)
    {
        _logger.LogInformation("Profile update request received for current user");

        try
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                _logger.LogWarning("Profile update failed. Current user was not found");

                return Unauthorized();
            }

            _logger.LogInformation(
                "Profile update started for user {UserId}",
                user.Id);

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
                _logger.LogWarning(
                    "Profile update failed for user {UserId}. Errors count: {ErrorsCount}",
                    user.Id,
                    result.Errors.Count());

                foreach (var error in result.Errors)
                {
                    _logger.LogWarning(
                        "Profile update validation error for user {UserId}. Code: {Code}, Description: {Description}",
                        user.Id,
                        error.Code,
                        error.Description);

                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem(ModelState);
            }

            var response = await CreateUserProfileResponseAsync(user);

            _logger.LogInformation(
                "Profile successfully updated for user {UserId}",
                user.Id);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating current user profile");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
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