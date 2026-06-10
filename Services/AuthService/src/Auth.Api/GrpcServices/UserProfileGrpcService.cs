using Auth.Api.Grpc;
using Auth.Domain.Entities;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using System;
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

        context.CancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            throw new RpcException(new Status(
                StatusCode.NotFound,
                "User was not found."));
        }

        return await CreateUserProfileGrpcResponseAsync(user);
    }

    public override async Task<GetUserProfilesByIdsResponse> GetUserProfilesByIds(
        GetUserProfilesByIdsRequest request,
        ServerCallContext context)
    {
        if (request.UserIds.Count == 0)
        {
            throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "UserIds collection is required."));
        }

        var uniqueUserIds = request.UserIds
            .Where(userId => !string.IsNullOrWhiteSpace(userId))
            .Select(userId => userId.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (uniqueUserIds.Count == 0)
        {
            throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "UserIds collection contains no valid values."));
        }

        var response = new GetUserProfilesByIdsResponse();

        foreach (var userId in uniqueUserIds)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                response.NotFoundUserIds.Add(userId);
                continue;
            }

            var profile = await CreateUserProfileGrpcResponseAsync(user);
            response.Profiles.Add(profile);
        }

        return response;
    }

    private async Task<UserProfileGrpcResponse> CreateUserProfileGrpcResponseAsync(User user)
    {
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