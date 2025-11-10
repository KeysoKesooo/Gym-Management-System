using GymManagement.Core.Models.AttendanceModel;

namespace GymManagement.Core.Repositories.IntAttendanceRepository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private static readonly List<Attendance> _attendances = new();

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            try
            {
                return await Task.FromResult(_attendances);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all attendances: " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByUserIdAsync(int userId)
        {
            try
            {
                var result = _attendances.Where(a => a.UserId == userId);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving attendances for user ID {userId}: {ex.Message}", ex);
            }
        }

        public async Task<Attendance> AddAttendanceAsync(Attendance attendance)
        {
            try
            {
                attendance.Id = _attendances.Count + 1;
                _attendances.Add(attendance);
                return await Task.FromResult(attendance);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding attendance for user ID {attendance.UserId}: {ex.Message}", ex);
            }
        }

        public async Task<Attendance?> UpdateAttendanceAsync(Attendance attendance)
        {
            try
            {
                var existing = _attendances.FirstOrDefault(a => a.Id == attendance.Id);
                if (existing == null)
                    return null;

                existing.CheckIn = attendance.CheckIn;
                existing.CheckOut = attendance.CheckOut;
                existing.UserId = attendance.UserId;

                return await Task.FromResult(existing);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating attendance with ID {attendance.Id}: {ex.Message}", ex);
            }
        }
    }
}
