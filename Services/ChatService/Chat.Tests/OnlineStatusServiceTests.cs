using Chat.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text;

namespace Chat.Tests
{
    public class OnlineStatusServiceTests
    {
        private readonly Mock<IDistributedCache> _cacheMock;
        private readonly OnlineStatusService _service;

        public OnlineStatusServiceTests()
        {
            _cacheMock = new Mock<IDistributedCache>();
            _service = new OnlineStatusService(_cacheMock.Object);
        }

        [Fact]
        public async Task SetOnlineAsync_ShouldCall_SetAsync_WithCorrectKeyAndTTL()
        {
            var userId = Guid.NewGuid();
            string expectedKey = $"user:online:{userId}";
            byte[] expectedValue = Encoding.UTF8.GetBytes("true");

            await _service.SetOnlineAsync(userId);

            _cacheMock.Verify(cache => cache.SetAsync(
                expectedKey,
                It.Is<byte[]>(v => v.SequenceEqual(expectedValue)),
                It.Is<DistributedCacheEntryOptions>(options =>
                    options.AbsoluteExpirationRelativeToNow == TimeSpan.FromSeconds(300)),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task SetOfflineAsync_ShouldCall_RemoveAsync_WithCorrectKey()
        {
            var userId = Guid.NewGuid();
            string expectedKey = $"user:online:{userId}";

            await _service.SetOfflineAsync(userId);

            _cacheMock.Verify(cache => cache.RemoveAsync(
                expectedKey,
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task IsOnlineAsync_ShouldReturnTrue_WhenKeyExists()
        {
            var userId = Guid.NewGuid();
            string expectedKey = $"user:online:{userId}";
            byte[] expectedValue = Encoding.UTF8.GetBytes("true");

            _cacheMock.Setup(cache => cache.GetAsync(
                expectedKey,
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(expectedValue);

            var result = await _service.IsOnlineAsync(userId);

            Assert.True(result);
            _cacheMock.Verify(cache => cache.GetAsync(
                expectedKey,
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task IsOnlineAsync_ShouldReturnFalse_WhenKeyDoesNotExist()
        {
            var userId = Guid.NewGuid();
            string expectedKey = $"user:online:{userId}";

            _cacheMock.Setup(cache => cache.GetAsync(
                expectedKey,
                It.IsAny<CancellationToken>()
            )).ReturnsAsync((byte[])null);

            var result = await _service.IsOnlineAsync(userId);

            Assert.False(result);
            _cacheMock.Verify(cache => cache.GetAsync(
                expectedKey,
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task IsOnlineAsync_MultipleUsers_ShouldReturnCorrectStatuses()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var userId3 = Guid.NewGuid();

            var userIds = new[] { userId1, userId2, userId3 };

            _cacheMock.Setup(cache => cache.GetAsync(
                $"user:online:{userId1}",
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(Encoding.UTF8.GetBytes("true"));

            _cacheMock.Setup(cache => cache.GetAsync(
                $"user:online:{userId2}",
                It.IsAny<CancellationToken>()
            )).ReturnsAsync((byte[])null);

            _cacheMock.Setup(cache => cache.GetAsync(
                $"user:online:{userId3}",
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(Encoding.UTF8.GetBytes("true"));

            var result = await _service.IsOnlineAsync(userIds);

            Assert.Equal(3, result.Count);
            Assert.True(result[userId1]);
            Assert.False(result[userId2]);
            Assert.True(result[userId3]);

            _cacheMock.Verify(cache => cache.GetAsync(
                $"user:online:{userId1}",
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _cacheMock.Verify(cache => cache.GetAsync(
                $"user:online:{userId2}",
                It.IsAny<CancellationToken>()
            ), Times.Once);

            _cacheMock.Verify(cache => cache.GetAsync(
                $"user:online:{userId3}",
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task SetOnlineAsync_ShouldOverrideExistingKey()
        {
            var userId = Guid.NewGuid();
            string expectedKey = $"user:online:{userId}";
            byte[] expectedValue = Encoding.UTF8.GetBytes("true");

            await _service.SetOnlineAsync(userId);
            await _service.SetOnlineAsync(userId);

            _cacheMock.Verify(cache => cache.SetAsync(
                expectedKey,
                It.Is<byte[]>(v => v.SequenceEqual(expectedValue)),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ), Times.Exactly(2));
        }

        [Fact]
        public async Task SetOfflineAsync_ShouldNotThrow_WhenKeyDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _cacheMock.Setup(cache => cache.RemoveAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            )).Returns(Task.CompletedTask);

            var exception = await Record.ExceptionAsync(() => _service.SetOfflineAsync(userId));
            Assert.Null(exception);

            _cacheMock.Verify(cache => cache.RemoveAsync(
                $"user:online:{userId}",
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
    }
}
