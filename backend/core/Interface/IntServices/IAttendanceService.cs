using System.Security.Claims;
using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Core.DTOs.AttendanceDto;

namespace GymManagement.Core.Services.IntAttendanceService
{
    public interface IAttendanceService
    {
        // ------------------------
        // READ METHODS
        // ------------------------
        Task<IEnumerable<Attendance>> GetAllAttendancesAsync(ClaimsPrincipal user);
        Task<IEnumerable<Attendance>> GetByUserIdAttendancesAsync(int userId, ClaimsPrincipal user);
        Task<IEnumerable<Attendance>> GetMyAttendanceAsync(ClaimsPrincipal user);

        // ------------------------
        // CHECK-IN / CHECK-OUT
        // ------------------------
        // Implements write-behind with cache + queue + TTL
        Task<(string message, List<AttendanceResponseDto> attendances)> CheckInOrOutAsync(ClaimsPrincipal user);

        // ------------------------
        // ADD / UPDATE ATTENDANCE
        // ------------------------
        // Implements write-behind with cache + TTL
        Task<Attendance> AddAttendanceAsync(Attendance attendance, ClaimsPrincipal user);
        Task<Attendance?> UpdateAttendanceAsync(Attendance attendance, ClaimsPrincipal user);
    }
}
