using GymManagement.Core.DTOs.AttendanceDto;
using GymManagement.Core.Models.AttendanceModel;
using GymManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.Core.Repositories.IntAttendanceRepository
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        // ------------------------
        // GET ALL ATTENDANCES
        // ------------------------
        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            try
            {
                return await _context.Attendances.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all attendances: " + ex.Message, ex);
            }
        }

        // ------------------------
        // GET ATTENDANCES BY USER
        // ------------------------
        public async Task<IEnumerable<Attendance>> GetAttendancesByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Attendances
                                     .Where(a => a.UserId == userId)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving attendances for user ID {userId}: {ex.Message}", ex);
            }
        }

        public async Task<Attendance> SaveCheckInAsync(AttendanceQueueModel model)
        {
            var attendance = new Attendance
            {
                UserId = model.UserId,
                CheckIn = model.Time
            };
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance; // DB id now available
        }

        public async Task SaveCheckOutAsync(AttendanceQueueModel model)
        {
            var existing = await _context.Attendances.FindAsync(model.AttendanceId);
            if (existing == null) throw new Exception($"Attendance ID {model.AttendanceId} not found");
            existing.CheckOut = model.Time;
            await _context.SaveChangesAsync();
        }


        // ------------------------
        // ADD ATTENDANCE (direct insert)
        // ------------------------
        public async Task<Attendance> AddAttendanceAsync(Attendance attendance)
        {
            try
            {
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
                return attendance;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding attendance for user ID {attendance.UserId}: {ex.Message}", ex);
            }
        }

        // ------------------------
        // UPDATE ATTENDANCE
        // ------------------------
        public async Task<Attendance?> UpdateAttendanceAsync(Attendance attendance)
        {
            try
            {
                var existing = await _context.Attendances.FindAsync(attendance.Id);
                if (existing == null) return null;

                existing.CheckIn = attendance.CheckIn;
                existing.CheckOut = attendance.CheckOut;
                existing.UserId = attendance.UserId;

                await _context.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating attendance with ID {attendance.Id}: {ex.Message}", ex);
            }
        }
    }
}
