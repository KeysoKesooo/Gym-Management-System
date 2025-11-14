using StackExchange.Redis;
using System.Text.Json;

namespace GymManagement.Core.Services.QueueService
{
    public class QueueService : IQueueService
    {
        private readonly IDatabase _redis;

        public QueueService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task EnqueueAsync<T>(string queue, T data)
        {
            var json = JsonSerializer.Serialize(data);
            await _redis.ListRightPushAsync(queue, json);
        }

        public async Task<T?> DequeueAsync<T>(string queue)
        {
            var data = await _redis.ListLeftPopAsync(queue);
            if (data.IsNullOrEmpty) return default;

            return JsonSerializer.Deserialize<T>(data!);
        }
    }
}
