using GymManagement.Core.Models.WalkinModel;
using GymManagement.Core.DTOs.WalkinDto;

namespace GymManagement.Core.Services.IntWalkinService
{
    public interface IWalkinService
    {
        Task<IEnumerable<WalkinGuest>> GetAllAsync();
        Task<WalkinGuest?> GetByIdAsync(int id);

        // Check-in / Check-out using write-behind queue
        Task<WalkinGuest> CheckInAsync(string name);
        Task<WalkinGuest> CheckOutAsync(int id);

        Task DeleteAsync(int id);
    }
}
