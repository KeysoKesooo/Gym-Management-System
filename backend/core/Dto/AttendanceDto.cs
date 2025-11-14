namespace GymManagement.Core.DTOs.AttendanceDto
{

    public class AttendanceResponseDto
    {
        public int Id { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int UserId { get; set; }
    }

    public class AttendanceCreateDto
    {
        public int UserId { get; set; }
    }
    public class AttendanceQueueDto
    {
        public int UserId { get; set; }
        public string Type { get; set; } = ""; // "checkin" or "checkout"
        public DateTime Time { get; set; }     // match model field
        public int? AttendanceId { get; set; } // needed for checkout
    }

}