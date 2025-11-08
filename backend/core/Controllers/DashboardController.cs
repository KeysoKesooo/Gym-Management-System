using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;

namespace GymManagement.Controllers.Dashboard
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // JWT required
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get dashboard stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            // Extract role from claims
            var role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value?.ToLower();

            if (role == null) return Unauthorized("Role not found");

            if (role == "admin")
            {
                // Admin sees all stats
                var membersInside = await _context.Attendances
                    .Where(a => a.CheckOut == null)
                    .CountAsync();

                var walkinsInside = await _context.WalkinGuests
                    .Where(g => g.CheckOut == null)
                    .CountAsync();

                return Ok(new
                {
                    membersInside,
                    walkinsInside,
                    totalInside = membersInside + walkinsInside,
                    timestamp = DateTime.UtcNow
                });
            }
            else if (role == "staff")
            {
                // Staff sees only walk-ins inside
                var walkinsInside = await _context.WalkinGuests
                    .Where(g => g.CheckOut == null)
                    .CountAsync();

                return Ok(new
                {
                    walkinsInside,
                    timestamp = DateTime.UtcNow
                });
            }

            return Forbid("Access denied for this role");
        }
    }
}
