using Chat.Application.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Chat.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly UserProfileGrpc.UserProfileGrpcClient _grpcClient;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(
            UserProfileGrpc.UserProfileGrpcClient grpcClient,
            ILogger<UserProfileService> logger)
        {
            _grpcClient = grpcClient;
            _logger = logger;
        }

        public async Task<Dictionary<Guid, (string SellerName, string SellerPhoto)>> GetUserInfosByIdsAsync(
            IEnumerable<Guid> userIds,
            CancellationToken cancellationToken = default)
        {
            if (userIds == null || !userIds.Any())
            {
                _logger.LogWarning("Empty or null user ids list provided");
                return [];
            }

            var results = new Dictionary<Guid, (string SellerName, string SellerPhoto)>();
            var failedIds = new List<Guid>();

            foreach (var userId in userIds)
            {
                try
                {
                    var request = new GetUserProfileByIdRequest
                    {
                        UserId = userId.ToString()
                    };

                    var response = await _grpcClient.GetUserProfileByIdAsync(
                        request,
                        deadline: DateTime.UtcNow.AddSeconds(5),
                        cancellationToken: cancellationToken);

                    var sellerName = $"{response.FirstName} {response.LastName}".Trim();
                    results.Add(userId, (sellerName, response.PhotoUrl));
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
                {
                    _logger.LogWarning(ex, "User profile not found for userId: {UserId}", userId);
                    failedIds.Add(userId);
                    continue;
                }
                catch (RpcException ex)
                {
                    _logger.LogError(ex, "gRPC error while fetching user profile for userId: {UserId}", userId);
                    throw new ApplicationException($"Failed to retrieve user profile for user {userId}: {ex.Status.Detail}", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while fetching user profile for userId: {UserId}", userId);
                    throw;
                }
            }

            if (failedIds.Count != 0)
            {
                _logger.LogWarning("Users not found: {FailedIds}", string.Join(", ", failedIds));
            }

            return results;
        }
    }
}
