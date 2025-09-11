using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;

namespace GymManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")] // only admin can view dashboard
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get real-time stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            // Count members currently inside
            var membersInside = await _context.Attendances
                .Where(a => a.CheckOut == null)
                .CountAsync();

            // Count walk-ins currently inside
            var walkinsInside = await _context.WalkinGuests
                .Where(g => g.CheckOut == null)
                .CountAsync();

            // Total inside
            var totalInside = membersInside + walkinsInside;

            return Ok(new
            {
                membersInside,
                walkinsInside,
                totalInside,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
