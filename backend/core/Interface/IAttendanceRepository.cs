using GymManagement.Core.Models.AttendanceModel;


namespace GymManagement.Core.Repositories.IntAttendanceRepository
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAllAttendancesAsync();
        Task<IEnumerable<Attendance>> GetAttendancesByUserIdAsync(int userId);
        Task<Attendance> AddAttendanceAsync(Attendance attendance);
    }
}