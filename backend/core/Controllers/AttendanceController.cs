
using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Core.Services.IntAttendanceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers.AttendanceController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // -----------------------
        // GET all attendances (Admin)
        // -----------------------
        [HttpGet]
        public async Task<IActionResult> GetAllAttendances()
        {
            try
            {
                var result = await _attendanceService.GetAllAttendancesAsync(User);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // -----------------------
        // GET attendances by user ID
        // -----------------------
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                var result = await _attendanceService.GetByUserIdAttendancesAsync(userId, User);
                if (!result.Any())
                    return NotFound(new { message = "No attendance records found for this user." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // -----------------------
        // GET current user's attendance
        // -----------------------
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAttendance()
        {
            try
            {
                var result = await _attendanceService.GetMyAttendanceAsync(User);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // -----------------------
        // Check-in / Check-out with caching and rate limiting
        // -----------------------
        [HttpPost("check")]
        public async Task<IActionResult> CheckInOrOut()
        {
            try
            {
                var (message, record) = await _attendanceService.CheckInOrOutAsync(User);
                return Ok(new { message, record });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // -----------------------
        // Optional: Check-in manually
        // -----------------------
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] Attendance request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request." });

            request.CheckIn = DateTime.UtcNow;
            var added = await _attendanceService.AddAttendanceAsync(request, User);
            return CreatedAtAction(nameof(GetByUserId), new { userId = added.UserId }, added);
        }

        // -----------------------
        // Optional: Check-out manually
        // -----------------------
        [HttpPut("checkout/{id}")]
        public async Task<IActionResult> CheckOut(int id)
        {
            try
            {
                var allAttendances = await _attendanceService.GetAllAttendancesAsync(User);
                var attendance = allAttendances.FirstOrDefault(a => a.Id == id);

                if (attendance == null)
                    return NotFound(new { message = "Attendance not found." });

                attendance.CheckOut = DateTime.UtcNow;
                var updated = await _attendanceService.UpdateAttendanceAsync(attendance, User);

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
