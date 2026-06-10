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
    private readonly ILogger<UserProfileGrpcService> _logger;

    public UserProfileGrpcService(
        UserManager<User> userManager,
        ILogger<UserProfileGrpcService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public override async Task<UserProfileGrpcResponse> GetUserProfileById(
        GetUserProfileByIdRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation(
            "gRPC request received for user profile {UserId}",
            request.UserId);

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            _logger.LogWarning(
                "gRPC request failed. UserId is empty");

            throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "UserId is required."));
        }

        context.CancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null)
        {
            _logger.LogWarning(
                "User profile {UserId} was not found by gRPC request",
                request.UserId);

            throw new RpcException(new Status(
                StatusCode.NotFound,
                "User was not found."));
        }

        var response = await CreateUserProfileGrpcResponseAsync(user);

        _logger.LogInformation(
            "User profile {UserId} successfully returned by gRPC",
            request.UserId);

        return response;
    }

    public override async Task<GetUserProfilesByIdsResponse> GetUserProfilesByIds(
        GetUserProfilesByIdsRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation(
            "gRPC request received for user profiles. Count: {Count}",
            request.UserIds.Count);

        if (request.UserIds.Count == 0)
        {
            _logger.LogWarning(
                "gRPC request failed. UserIds collection is empty");

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
            _logger.LogWarning(
                "gRPC request failed. UserIds collection contains no valid values. Requested count: {RequestedCount}",
                request.UserIds.Count);

            throw new RpcException(new Status(
                StatusCode.InvalidArgument,
                "UserIds collection contains no valid values."));
        }

        _logger.LogInformation(
            "gRPC user profiles request prepared. Requested: {RequestedCount}, Valid unique ids: {ValidUniqueCount}",
            request.UserIds.Count,
            uniqueUserIds.Count);

        var response = new GetUserProfilesByIdsResponse();

        foreach (var userId in uniqueUserIds)
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                _logger.LogWarning(
                    "User profile {UserId} was not found by gRPC batch request",
                    userId);

                response.NotFoundUserIds.Add(userId);
                continue;
            }

            var profile = await CreateUserProfileGrpcResponseAsync(user);
            response.Profiles.Add(profile);
        }

        _logger.LogInformation(
            "gRPC user profiles response prepared. Requested: {RequestedCount}, Valid unique ids: {ValidUniqueCount}, Found: {FoundCount}, Not found: {NotFoundCount}",
            request.UserIds.Count,
            uniqueUserIds.Count,
            response.Profiles.Count,
            response.NotFoundUserIds.Count);

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