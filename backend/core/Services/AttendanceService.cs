using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Core.DTOs.UserDto;
using GymManagement.Core.Repositories.IntAttendanceRepository;

namespace GymManagement.Core.Services.IntAttendanceService
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _repo;

        public AttendanceService(IAttendanceRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            return await _repo.GetAllAttendancesAsync();
        }

        public async Task<IEnumerable<Attendance>> GetByUserIdAttendancesAsync(int userId)
        {
            return await _repo.GetAttendancesByUserIdAsync(userId);
        }

        public async Task<Attendance> AddAsync(Attendance attendance)
        {
            return await _repo.AddAttendanceAsync(attendance);
        }

        // Optional: DTO mapping
        public async Task<IEnumerable<AttendanceResponseDto>> GetByUserIdDtoAsync(int userId)
        {
            var attendances = await _repo.GetAttendancesByUserIdAsync(userId);
            return attendances.Select(a => new AttendanceResponseDto
            {
                Id = a.Id,
                UserId = a.UserId,
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut
            });
        }
    }
}
