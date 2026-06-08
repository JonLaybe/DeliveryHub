using Auth.Api.Grpc;
using Auth.Domain.Entities;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Auth.Api.GrpcServices;

public sealed class UserProfileGrpcService : UserProfileGrpc.UserProfileGrpcBase
{
    private readonly UserManager<User> _userManager;

    public UserProfileGrpcService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public override async Task<UserProfileGrpcResponse> GetUserProfileById(
        GetUserProfileByIdRequest request,
        ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "UserId is required."));
        }

        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            throw new RpcException(new Status(
                StatusCode.NotFound,
                "User was not found."));
        }

        var roles = await _userManager.GetRolesAsync(user);

        var response = new UserProfileGrpcResponse
        {
            Id = user.Id.ToString(),
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            PhotoUrl = user.PhotoUrl ?? string.Empty,
            BirthDate = user.BirthDate?.ToString("yyyy-MM-dd") ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Country = user.Country ?? string.Empty,
            City = user.City ?? string.Empty
        };

        response.Roles.AddRange(roles);

        return response;
    }
}