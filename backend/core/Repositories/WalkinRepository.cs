using GymManagement.Core.Models.WalkinModel;
using GymManagement.Data;
using Microsoft.EntityFrameworkCore;
using GymManagement.Core.DTOs.WalkinDto;

namespace GymManagement.Core.Repositories.IntWalkinRepository
{
    public class WalkinRepository : IWalkinRepository
    {
        private readonly AppDbContext _context;

        public WalkinRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WalkinGuest>> GetAllAsync()
        {
            return await _context.WalkinGuests.ToListAsync();
        }

        public async Task<WalkinGuest?> GetByIdAsync(int id)
        {
            return await _context.WalkinGuests.FindAsync(id);
        }

        public async Task<WalkinGuest> AddAsync(WalkinGuest guest)
        {
            _context.WalkinGuests.Add(guest);
            await _context.SaveChangesAsync();
            return guest;
        }

        public async Task UpdateAsync(WalkinGuest guest)
        {
            var existing = await _context.WalkinGuests.FindAsync(guest.Id);
            if (existing == null)
                throw new Exception($"Walk-in guest ID {guest.Id} not found");

            existing.Name = guest.Name;
            existing.CheckIn = guest.CheckIn;
            existing.CheckOut = guest.CheckOut;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var guest = await _context.WalkinGuests.FindAsync(id);
            if (guest == null)
                throw new Exception("Walk-in guest not found");

            _context.WalkinGuests.Remove(guest);
            await _context.SaveChangesAsync();
        }

        // ------------------------
        // Write-behind queue processing
        // ------------------------
        public async Task SaveFromQueueAsync(WalkinQueueDto queueItem)
        {
            if (queueItem.Type == "checkin")
            {
                var guest = new WalkinGuest
                {
                    Name = queueItem.Name ?? "",
                    CheckIn = queueItem.Time
                };
                await AddAsync(guest);
            }
            else if (queueItem.Type == "checkout")
            {
                var guest = await GetByIdAsync(queueItem.GuestId);
                if (guest == null)
                    throw new Exception($"Walk-in guest ID {queueItem.GuestId} not found");

                guest.CheckOut = queueItem.Time;
                await UpdateAsync(guest);
            }
        }
    }
}
