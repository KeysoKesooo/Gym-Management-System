using GymManagement.Core.Models.UserModel;

namespace GymManagement.Core.Models.AttendanceModel
{
    public class Attendance
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public required User User { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }
}
