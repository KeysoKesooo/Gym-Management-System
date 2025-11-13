using StackExchange.Redis;
using System.Text.Json;

namespace GymManagement.Core.Services.RedisService
{
    public class RedisSessionService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisSessionService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        // Save session in Redis
        public async Task SetSessionAsync<T>(string token, T data, TimeSpan ttl)
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(data);
            await db.StringSetAsync(token, json, ttl);
        }

        // Get session from Redis
        public async Task<T?> GetSessionAsync<T>(string token)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(token);

            if (value.IsNullOrEmpty)
                return default;

            // ✅ Convert RedisValue to string and safely deserialize
            var json = (string?)value;
            if (json == null)
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }

        // Remove session from Redis
        public async Task RemoveSessionAsync(string token)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(token);
        }
    }
}
