using Chat.Application;
using Chat.Application.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chat.Tests
{
    public class UserProfileServiceTests
    {
        private readonly Mock<UserProfileGrpc.UserProfileGrpcClient> _grpcClientMock;
        private readonly Mock<ILogger<UserProfileService>> _loggerMock;
        private readonly UserProfileService _service;

        public UserProfileServiceTests()
        {
            _grpcClientMock = new Mock<UserProfileGrpc.UserProfileGrpcClient>();
            _loggerMock = new Mock<ILogger<UserProfileService>>();
            _service = new UserProfileService(_grpcClientMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserInfosByIdsAsync_ShouldReturnUserInfos_WhenAllUsersExist()
        {
            var userIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            var responses = new Dictionary<Guid, UserProfileGrpcResponse>();

            foreach (var userId in userIds)
            {
                var response = new UserProfileGrpcResponse
                {
                    Id = userId.ToString(),
                    FirstName = $"User{userId}",
                    LastName = $"LastName{userId}",
                    PhotoUrl = $"http://photo.com/{userId}.jpg"
                };
                responses[userId] = response;

                SetupGrpcCall(userId.ToString(), response);
            }

            var result = await _service.GetUserInfosByIdsAsync(userIds);

            Assert.Equal(2, result.Count);
            foreach (var userId in userIds)
            {
                Assert.Contains(userId, result);
                Assert.Equal($"User{userId} LastName{userId}", result[userId].SellerName);
                Assert.Equal($"http://photo.com/{userId}.jpg", result[userId].SellerPhoto);
            }
        }

        [Fact]
        public async Task GetUserInfosByIdsAsync_ShouldSkipNotFoundUsers_AndReturnOnlyExisting()
        {
            var existingUserId = Guid.NewGuid();
            var notFoundUserId = Guid.NewGuid();
            var userIds = new List<Guid> { existingUserId, notFoundUserId };

            var existingResponse = new UserProfileGrpcResponse
            {
                Id = existingUserId.ToString(),
                FirstName = "John",
                LastName = "Doe",
                PhotoUrl = "http://photo.com/john.jpg"
            };

            await SetupGrpcCall(existingUserId.ToString(), existingResponse);
            SetupGrpcCallWithError(notFoundUserId.ToString(), new RpcException(new Status(StatusCode.NotFound, "Not found")));

            var result = await _service.GetUserInfosByIdsAsync(userIds);

            Assert.Single(result);
            Assert.Contains(existingUserId, result);
            Assert.DoesNotContain(notFoundUserId, result);
            Assert.Equal("John Doe", result[existingUserId].SellerName);
        }

        [Fact]
        public async Task GetUserInfosByIdsAsync_ShouldReturnEmptyDictionary_WhenUserIdsIsNull()
        {
            var result = await _service.GetUserInfosByIdsAsync(null!);

            Assert.Empty(result);
            _grpcClientMock.Verify(x => x.GetUserProfileByIdAsync(It.IsAny<GetUserProfileByIdRequest>(), null, null, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetUserInfosByIdsAsync_ShouldReturnEmptyDictionary_WhenUserIdsIsEmpty()
        {
            var result = await _service.GetUserInfosByIdsAsync(new List<Guid>());

            Assert.Empty(result);
            _grpcClientMock.Verify(x => x.GetUserProfileByIdAsync(It.IsAny<GetUserProfileByIdRequest>(), null, null, It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetUserInfosByIdsAsync_ShouldThrowApplicationException_WhenGrpcErrorOccurs()
        {
            var userId = Guid.NewGuid();
            var userIds = new List<Guid> { userId };

            SetupGrpcCallWithError(userId.ToString(), new RpcException(new Status(StatusCode.Internal, "Internal server error")));

            var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
                _service.GetUserInfosByIdsAsync(userIds));

            Assert.Contains($"Failed to retrieve user profile for user {userId}", exception.Message);
        }

        private async Task SetupGrpcCall(string userId, UserProfileGrpcResponse response)
        {
            var asyncUnaryCall = new AsyncUnaryCall<UserProfileGrpcResponse>(
                Task.FromResult(response),
                null,
                null,
                null,
                () => { });

            _grpcClientMock
                .Setup(x => x.GetUserProfileByIdAsync(
                    It.Is<GetUserProfileByIdRequest>(r => r.UserId == userId),
                    null,
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);

            await Task.CompletedTask;
        }

        private void SetupGrpcCallWithError(string userId, Exception exception)
        {
            var asyncUnaryCall = new AsyncUnaryCall<UserProfileGrpcResponse>(
                Task.FromException<UserProfileGrpcResponse>(exception),
                null,
                null,
                null,
                () => { });

            _grpcClientMock
                .Setup(x => x.GetUserProfileByIdAsync(
                    It.Is<GetUserProfileByIdRequest>(r => r.UserId == userId),
                    null,
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(asyncUnaryCall);
        }
    }
}
