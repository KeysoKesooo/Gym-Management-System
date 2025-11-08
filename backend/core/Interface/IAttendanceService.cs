using GymManagement.Core.Models.AttendanceModel;

namespace GymManagement.Core.Services.IntAttendanceService

{
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> GetAllAttendancesAsync();
        Task<IEnumerable<Attendance>> GetByUserIdAttendancesAsync(int userId);
        Task<Attendance> AddAsync(Attendance attendance);
    }
}
