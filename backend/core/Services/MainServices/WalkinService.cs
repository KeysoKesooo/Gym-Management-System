using GymManagement.Core.Models.WalkinModel;
using GymManagement.Core.Repositories.IntWalkinRepository;
using GymManagement.Core.DTOs.WalkinDto;
using GymManagement.Core.Services.QueueService;

namespace GymManagement.Core.Services.IntWalkinService
{
    public class WalkinService : IWalkinService
    {
        private readonly IWalkinRepository _repo;
        private readonly IQueueService _queue;
        private readonly TimeSpan _ttl = TimeSpan.FromHours(8); // Cache/TTL for walk-ins

        public WalkinService(IWalkinRepository repo, IQueueService queue)
        {
            _repo = repo;
            _queue = queue;
        }

        public async Task<IEnumerable<WalkinGuest>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<WalkinGuest?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<WalkinGuest> CheckInAsync(string name)
        {
            // 1️⃣ Create DTO for queue
            var queueItem = new WalkinQueueDto
            {
                Type = "checkin",
                Name = name,
                Time = DateTime.UtcNow
            };

            // 2️⃣ Enqueue for background save
            await _queue.EnqueueAsync("walkin_queue", queueItem);

            // 3️⃣ Return temp guest object immediately
            return new WalkinGuest
            {
                Name = name,
                CheckIn = queueItem.Time
            };
        }

        public async Task<WalkinGuest> CheckOutAsync(int id)
        {
            var queueItem = new WalkinQueueDto
            {
                Type = "checkout",
                GuestId = id,
                Time = DateTime.UtcNow
            };

            await _queue.EnqueueAsync("walkin_queue", queueItem);

            // Return temp object with checkout timestamp
            return new WalkinGuest
            {
                Id = id,
                CheckOut = queueItem.Time
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
