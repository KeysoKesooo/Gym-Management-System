using System.Security.Claims;
using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Core.DTOs.AttendanceDto;
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
            try
            {
                return await _repo.GetAllAttendancesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all attendances: " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<Attendance>> GetByUserIdAttendancesAsync(int userId)
        {
            try
            {
                return await _repo.GetAttendancesByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving attendances for user ID {userId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Attendance>> GetMyAttendanceAsync(ClaimsPrincipal user)
        {
            try
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                    throw new UnauthorizedAccessException("Invalid or missing user ID in token.");

                // Get attendances from repo
                var attendances = await _repo.GetAttendancesByUserIdAsync(userId);

                // Return empty list if none found
                return attendances ?? Enumerable.Empty<Attendance>();
            }
            catch (UnauthorizedAccessException)
            {
                // Let controller handle unauthorized separately
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving current user's attendances: " + ex.Message, ex);
            }
        }


        public async Task<(string message, AttendanceResponseDto attendance)> CheckInOrOutAsync(ClaimsPrincipal user)
        {
            try
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
                    throw new UnauthorizedAccessException("Invalid or missing user ID in token.");

                var attendances = await _repo.GetAttendancesByUserIdAsync(userId);
                var last = attendances.OrderByDescending(a => a.CheckIn).FirstOrDefault();

                Attendance current;
                string message;

                if (last == null || last.CheckOut != null)
                {
                    // ✅ Check-in
                    current = new Attendance { UserId = userId, CheckIn = DateTime.UtcNow };
                    current = await _repo.AddAttendanceAsync(current);
                    message = "Checked in successfully!";
                }
                else
                {
                    // ✅ Check-out
                    last.CheckOut = DateTime.UtcNow;
                    current = await _repo.UpdateAttendanceAsync(last) ?? last;
                    message = "Checked out successfully!";
                }

                var dto = new AttendanceResponseDto
                {
                    Id = current.Id,
                    UserId = current.UserId,
                    CheckIn = current.CheckIn,
                    CheckOut = current.CheckOut
                };

                return (message, dto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during check-in/check-out: " + ex.Message, ex);
            }
        }

        public async Task<Attendance> AddAttendanceAsync(Attendance attendance)
        {
            try
            {
                return await _repo.AddAttendanceAsync(attendance);
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
                return await _repo.UpdateAttendanceAsync(attendance);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating attendance with ID {attendance.Id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<AttendanceResponseDto>> GetByUserIdDtoAsync(int userId)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving attendance DTOs for user ID {userId}: {ex.Message}", ex);
            }
        }
    }
}
