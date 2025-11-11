using GymManagement.Core.Models.WalkinModel;
using GymManagement.Core.Repositories.IntWalkinRepository;


namespace GymManagement.Core.Services.IntWalkinService
{
    public class WalkinService : IWalkinService
    {
        private readonly IWalkinRepository _repo;

        public WalkinService(IWalkinRepository repo)
        {
            _repo = repo;
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
            var guest = new WalkinGuest
            {
                Name = name,
                CheckIn = DateTime.Now
            };
            return await _repo.AddAsync(guest);
        }

        public async Task<WalkinGuest> CheckOutAsync(int id)
        {
            var guest = await _repo.GetByIdAsync(id)
                ?? throw new Exception("Guest not found");

            guest.CheckOut = DateTime.Now;
            await _repo.UpdateAsync(guest);

            return guest;
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
