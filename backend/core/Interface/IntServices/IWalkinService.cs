using GymManagement.Core.Models.WalkinModel;

namespace GymManagement.Core.Services.IntWalkinService
{
    public interface IWalkinService
    {
        Task<IEnumerable<WalkinGuest>> GetAllAsync();
        Task<WalkinGuest?> GetByIdAsync(int id);
        Task<WalkinGuest> CheckInAsync(string name);
        Task<WalkinGuest> CheckOutAsync(int id);
        Task DeleteAsync(int id);
    }
}
