using Chat.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Application.Services
{
    public class JwksManager : IJwksManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<JwksManager> _logger;
        private JsonWebKeySet? _cachedJwks;
        private readonly Lock _lock = new();
        private DateTime _lastFetch = DateTime.MinValue;
        private readonly TimeSpan _cacheTtl = TimeSpan.FromHours(24);

        public JwksManager(
            IConfiguration configuration, 
            ILogger<JwksManager> logger, 
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IEnumerable<SecurityKey> GetKeys(string? kid)
        {
            EnsureJwksLoaded();

            if (_cachedJwks == null)
                return [];

            if (!string.IsNullOrEmpty(kid))
            {
                var key = _cachedJwks.Keys.FirstOrDefault(k => k.Kid == kid);
                if (key != null)
                    return [key];
            }

            return _cachedJwks.Keys;
        }

        private void EnsureJwksLoaded()
        {
            if (_cachedJwks != null && DateTime.UtcNow - _lastFetch <= _cacheTtl)
                return;

            lock (_lock)
            {
                if (_cachedJwks != null && DateTime.UtcNow - _lastFetch <= _cacheTtl)
                    return;

                try
                {
                    var jwksUrl = _configuration["JWTSettings:JwksUrl"];
                    if (jwksUrl == null)
                    {
                        throw new InvalidOperationException("Set up url for jwks in appsettings.json!");
                    }

                    _logger.LogInformation("Loading JWKS from {Url}", jwksUrl);

                    var client = _httpClientFactory.CreateClient();
                    client.Timeout = TimeSpan.FromSeconds(10);

                    var jwksJson = client.GetStringAsync(jwksUrl).GetAwaiter().GetResult();
                    _cachedJwks = new JsonWebKeySet(jwksJson);
                    _lastFetch = DateTime.UtcNow;

                    _logger.LogInformation("JWKS loaded successfully. Keys count: {Count}",
                        _cachedJwks.Keys.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load JWKS");
                    throw;
                }
            }
        }
    }
}
