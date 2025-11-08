using GymManagement.Core.Models.AttendanceModel;



namespace GymManagement.Core.Repositories.IntAttendanceRepository
{

    public class AttendanceRepository : IAttendanceRepository
    {
        private static readonly List<Attendance> _attendances = new();

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync() => await Task.FromResult(_attendances);
        public async Task<IEnumerable<Attendance>> GetAttendancesByUserIdAsync(int userId) =>
            await Task.FromResult(_attendances.Where(a => a.UserId == userId));
        public async Task<Attendance> AddAttendanceAsync(Attendance attendance)
        {
            attendance.Id = _attendances.Count + 1;
            _attendances.Add(attendance);
            return await Task.FromResult(attendance);
        }
    }

}