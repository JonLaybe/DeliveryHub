using Chat.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Chat.Application.Services
{
    public class OnlineStatusService : IOnlineStatusService
    {
        private readonly IDistributedCache _distributedCache;
        private const int OnlineTtlSeconds = 300;
        private const string Key = "user:online:";

        public OnlineStatusService(
            IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SetOnlineAsync(Guid userId)
        {
            var key = GetKey(userId);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(OnlineTtlSeconds)
            };
            await _distributedCache.SetStringAsync(key, "true", options);
        }

        public async Task SetOfflineAsync(Guid userId)
        {
            var key = GetKey(userId);
            await _distributedCache.RemoveAsync(key);
        }

        public async Task<bool> IsOnlineAsync(Guid userId)
        {
            var key = GetKey(userId);
            var value = await _distributedCache.GetStringAsync(key);
            if (value != null)
            {
                return true;
            }
            return false;
        }

        public async Task<Dictionary<Guid, bool>> IsOnlineAsync(IEnumerable<Guid> userIds)
        {
            var tasks = userIds.Select(async userId => new
            {
                UserId = userId,
                IsOnline = await IsOnlineAsync(userId)
            });

            var results = await Task.WhenAll(tasks);

            return results.ToDictionary(x => x.UserId, x => x.IsOnline);
        }

        private static string GetKey(Guid userId) => $"{Key}{userId}";
    }
}
