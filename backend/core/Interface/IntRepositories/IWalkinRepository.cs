using GymManagement.Core.Models.WalkinModel;

namespace GymManagement.Core.Repositories.IntWalkinRepository
{
    public interface IWalkinRepository
    {
        Task<IEnumerable<WalkinGuest>> GetAllAsync();
        Task<WalkinGuest?> GetByIdAsync(int id);
        Task<WalkinGuest> AddAsync(WalkinGuest guest);
        Task UpdateAsync(WalkinGuest guest);
        Task DeleteAsync(int id);
    }
}
