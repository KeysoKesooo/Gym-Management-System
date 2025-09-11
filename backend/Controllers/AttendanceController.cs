using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GymManagement.Data;
using GymManagement.Models;

namespace GymManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // require JWT authentication
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get my attendance records (members only)
        [HttpGet("my")]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> GetMyAttendance()
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "id").Value);

            var records = await _context.Attendances
                .Where(a => a.UserId == userId)
                .Select(a => new
                {
                    a.Id,
                    a.CheckIn,
                    a.CheckOut
                })
                .ToListAsync();

            return Ok(records);
        }


        // ✅ Check-in member
        [HttpPost("checkin/{userId}")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> CheckIn(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found");

            bool alreadyInside = await _context.Attendances
                .AnyAsync(a => a.UserId == userId && a.CheckOut == null);
            if (alreadyInside) return BadRequest($"{user.Name} is already checked in");

            var attendance = new Attendance
            {
                UserId = userId,
                User = user,
                CheckIn = DateTime.UtcNow
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{user.Name} checked in successfully" });
        }

        // ✅ Check-out member
        [HttpPost("checkout/{userId}")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> CheckOut(int userId)
        {
            var attendance = await _context.Attendances
                .Include(a => a.User)
                .Where(a => a.UserId == userId && a.CheckOut == null)
                .FirstOrDefaultAsync();

            if (attendance == null) return BadRequest("User is not currently inside");

            attendance.CheckOut = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{attendance.User.Name} checked out successfully" });
        }

        // ✅ Who is currently inside (members only)
        [HttpGet("inside")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> GetCurrentlyInside()
        {
            var insideUsers = await _context.Attendances
                .Include(a => a.User)
                .Where(a => a.CheckOut == null)
                .Select(a => new
                {
                    a.User.Id,
                    a.User.Name,
                    a.User.Role,
                    a.CheckIn
                })
                .ToListAsync();

            return Ok(new { count = insideUsers.Count, people = insideUsers });
        }

        // ✅ Walk-in check-in
        [HttpPost("walkin/checkin")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> WalkInCheckIn([FromBody] WalkinRequest request)
        {
            var guest = new WalkinGuest
            {
                Name = request.Name,
                CheckIn = DateTime.UtcNow
            };

            _context.WalkinGuests.Add(guest);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Walk-in guest {request.Name} checked in" });
        }

        // ✅ Walk-in check-out
        [HttpPost("walkin/checkout/{id}")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> WalkInCheckOut(int id)
        {
            var guest = await _context.WalkinGuests
                .Where(g => g.Id == id && g.CheckOut == null)
                .FirstOrDefaultAsync();

            if (guest == null) return BadRequest("Walk-in guest not found or already checked out");

            guest.CheckOut = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Walk-in guest {guest.Name} checked out" });
        }

        // ✅ See all walk-ins currently inside
        [HttpGet("walkin/inside")]
        [Authorize(Roles = "staff,admin")]
        public async Task<IActionResult> WalkInsInside()
        {
            var inside = await _context.WalkinGuests
                .Where(g => g.CheckOut == null)
                .Select(g => new { g.Id, g.Name, g.CheckIn })
                .ToListAsync();

            return Ok(new { count = inside.Count, guests = inside });
        }
    }

    public class WalkinRequest
    {
        public required string Name { get; set; }
    }
}
