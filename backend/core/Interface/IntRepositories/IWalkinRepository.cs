using GymManagement.Core.Models.WalkinModel;
using GymManagement.Core.DTOs.WalkinDto;

namespace GymManagement.Core.Repositories.IntWalkinRepository
{
    public interface IWalkinRepository
    {
        Task<IEnumerable<WalkinGuest>> GetAllAsync();
        Task<WalkinGuest?> GetByIdAsync(int id);
        Task<WalkinGuest> AddAsync(WalkinGuest guest);
        Task UpdateAsync(WalkinGuest guest);
        Task DeleteAsync(int id);

        // Queue write-behind using DTO
        Task SaveFromQueueAsync(WalkinQueueDto queueItem);
    }
}
