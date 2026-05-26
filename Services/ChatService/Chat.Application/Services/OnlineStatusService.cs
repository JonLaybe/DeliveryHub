using Chat.Application.Interfaces;
using StackExchange.Redis;

namespace Chat.Application.Services
{
    public class OnlineStatusService : IOnlineStatusService
    {
        private readonly IDatabase _db;
        private const int OnlineTtlSeconds = 300;
        private const string Key = "user:online:";

        public OnlineStatusService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SetOnlineAsync(Guid userId)
        {
            var key = GetKey(userId);
            await _db.StringSetAsync(key, true, TimeSpan.FromSeconds(OnlineTtlSeconds));
        }

        public async Task SetOfflineAsync(Guid userId)
        {
            var key = GetKey(userId);
            await _db.KeyDeleteAsync(key);
        }

        public async Task<bool> IsOnlineAsync(Guid userId)
        {
            var key = GetKey(userId);
            return await _db.KeyExistsAsync(key);
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
