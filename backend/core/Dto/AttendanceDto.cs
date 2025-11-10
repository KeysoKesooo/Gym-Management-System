

namespace GymManagement.Core.DTOs.AttendanceDto;

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