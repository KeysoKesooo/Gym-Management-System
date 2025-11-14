using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GymManagement.Core.Models.WalkinModel;
using GymManagement.Core.Repositories.IntWalkinRepository;
using GymManagement.Core.Services.QueueService;
using GymManagement.Core.DTOs.WalkinDto;

namespace GymManagement.Core.Workers.WalkinWorker
{
    public class WalkinWorker : BackgroundService
    {
        private readonly ILogger<WalkinWorker> _logger;
        private readonly IQueueService _queue;
        private readonly IServiceProvider _provider;

        public WalkinWorker(ILogger<WalkinWorker> logger, IQueueService queue, IServiceProvider provider)
        {
            _logger = logger;
            _queue = queue;
            _provider = provider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var item = await _queue.DequeueAsync<WalkinQueueDto>("walkin_queue");

                    if (item == null)
                    {
                        await Task.Delay(500, stoppingToken); // No item, wait a bit
                        continue;
                    }

                    using var scope = _provider.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IWalkinRepository>();

                    if (item.Type == "checkin")
                    {
                        var guest = new WalkinGuest
                        {
                            Name = item.Name,
                            CheckIn = item.Time
                        };

                        await repo.AddAsync(guest);
                    }
                    else if (item.Type == "checkout")
                    {
                        var guest = await repo.GetByIdAsync(item.GuestId);
                        if (guest == null)
                        {
                            _logger.LogWarning("Guest ID {GuestId} not found for checkout", item.GuestId);
                            continue;
                        }

                        guest.CheckOut = item.Time;
                        await repo.UpdateAsync(guest);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process walkin queue item");
                }
            }
        }
    }
}
