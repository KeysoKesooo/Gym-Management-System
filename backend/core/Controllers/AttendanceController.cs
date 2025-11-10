using System.Security.Claims;
using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Core.Services.IntAttendanceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers.AttendanceController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // ✅ Protects all endpoints with JWT
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // 🔹 GET: api/attendance
        [HttpGet]
        public async Task<IActionResult> GetAllAttendances()
        {
            var result = await _attendanceService.GetAllAttendancesAsync();
            return Ok(result);
        }

        // 🔹 GET: api/attendance/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var result = await _attendanceService.GetByUserIdAttendancesAsync(userId);
            if (!result.Any())
                return NotFound(new { message = "No attendance records found for this user." });

            return Ok(result);
        }

        // ✅ GET: api/attendance/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAttendance()
        {
            try
            {
                var result = await _attendanceService.GetMyAttendanceAsync(User);
                return Ok(result); // Always 200 OK, even if empty
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


        // ✅ POST: api/attendance/check
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
        }

        // 🔹 POST: api/attendance/checkin
        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] Attendance request)
        {
            if (request == null)
                return BadRequest(new { message = "Invalid request." });

            request.CheckIn = DateTime.UtcNow;
            var added = await _attendanceService.AddAttendanceAsync(request);
            return CreatedAtAction(nameof(GetByUserId), new { userId = added.UserId }, added);
        }

        // 🔹 PUT: api/attendance/checkout/{id}
        [HttpPut("checkout/{id}")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var allAttendances = await _attendanceService.GetAllAttendancesAsync();
            var attendance = allAttendances.FirstOrDefault(a => a.Id == id);

            if (attendance == null)
                return NotFound(new { message = "Attendance not found." });

            attendance.CheckOut = DateTime.UtcNow;

            var updated = await _attendanceService.UpdateAttendanceAsync(attendance);
            return Ok(updated);
        }

    }
}
