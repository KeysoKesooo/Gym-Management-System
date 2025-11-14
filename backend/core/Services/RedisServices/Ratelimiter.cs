using StackExchange.Redis;

namespace GymManagement.Core.Services.RatelimiterService
{
    public class RedisRateLimiter
    {
        private readonly IDatabase _db;

        public RedisRateLimiter(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        /// <summary>
        /// Checks if action is allowed based on rate limit per key.
        /// </summary>
        /// <param name="key">Unique key per user/action</param>
        /// <param name="limit">Max allowed requests</param>
        /// <param name="period">Time window</param>
        /// <returns>True if allowed, false if exceeded</returns>
        public async Task<bool> IsAllowedAsync(string key, int limit, TimeSpan period)
        {
            var current = await _db.StringIncrementAsync(key);

            // Set expiration only on first request
            if (current == 1)
                await _db.KeyExpireAsync(key, period);

            return current <= limit;
        }

        /// <summary>
        /// Optional: Reset rate limit manually (for testing or admin override)
        /// </summary>
        public async Task ResetAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
