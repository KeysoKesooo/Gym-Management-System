using System.Security.Claims;
using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Core.DTOs.AttendanceDto;

namespace GymManagement.Core.Services.IntAttendanceService

{
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> GetAllAttendancesAsync();
        Task<IEnumerable<Attendance>> GetByUserIdAttendancesAsync(int userId);
        Task<Attendance> AddAttendanceAsync(Attendance attendance);
        Task<Attendance?> UpdateAttendanceAsync(Attendance attendance);
        Task<IEnumerable<Attendance>> GetMyAttendanceAsync(ClaimsPrincipal user);
        Task<(string message, AttendanceResponseDto attendance)> CheckInOrOutAsync(ClaimsPrincipal user);
    }
}
