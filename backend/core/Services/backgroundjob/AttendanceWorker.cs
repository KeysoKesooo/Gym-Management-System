using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GymManagement.Core.DTOs.AttendanceDto;
using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Core.Services.QueueService;
using GymManagement.Core.Repositories.IntAttendanceRepository;
using Microsoft.Extensions.DependencyInjection;
using GymManagement.Core.Services.CacheService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagement.Core.Workers.AttendanceWorker
{
    public class AttendanceWorker : BackgroundService
    {
        private readonly ILogger<AttendanceWorker> _logger;
        private readonly IQueueService _queue;
        private readonly IServiceProvider _provider;
        private readonly RedisCacheService _cache;

        public AttendanceWorker(
            ILogger<AttendanceWorker> logger,
            IQueueService queue,
            IServiceProvider provider,
            RedisCacheService cache)
        {
            _logger = logger;
            _queue = queue;
            _provider = provider;
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var item = await _queue.DequeueAsync<AttendanceQueueModel>("attendance_queue");

                if (item != null)
                {
                    try
                    {
                        using var scope = _provider.CreateScope();
                        var repo = scope.ServiceProvider.GetRequiredService<IAttendanceRepository>();

                        // Save to DB
                        if (item.Type == "checkin")
                        {
                            var saved = await repo.SaveCheckInAsync(item);
                            // Update pending check-in cache with real DB id
                            await _cache.SetCacheAsync($"pending_checkin:{item.UserId}", saved.Id, TimeSpan.FromMinutes(10));
                        }
                        else
                        {
                            await repo.SaveCheckOutAsync(item);
                            // Clear pending check-in after checkout
                            await _cache.RemoveCacheAsync($"pending_checkin:{item.UserId}");
                        }

                        _logger.LogInformation("Processed attendance for user {UserId}", item.UserId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed queue item, retrying...");
                        await Task.Delay(1000, stoppingToken);
                        await _queue.EnqueueAsync("attendance_queue", item); // retry
                    }
                }
                else
                {
                    await Task.Delay(500, stoppingToken);
                }
            }
        }
    }
}
