using StackExchange.Redis;
using System.Text.Json;

namespace GymManagement.Core.Services.CacheService;

public class RedisCacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task SetCacheAsync<T>(string key, T value, TimeSpan expiry)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetCacheAsync<T>(string key)
    {
        var json = await _db.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            return default;

        // Convert RedisValue to string safely
        var jsonString = (string?)json;
        if (jsonString == null)
            return default;

        // Deserialize safely
        return JsonSerializer.Deserialize<T>(jsonString);
    }


    public async Task RemoveCacheAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}
