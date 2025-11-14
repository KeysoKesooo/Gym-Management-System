using System;

namespace GymManagement.Core.DTOs.WalkinDto
{
    public class WalkinResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }

    public class WalkinCreateDto
    {
        public string Name { get; set; } = "";
    }

    public class WalkinQueueDto
    {
        public int GuestId { get; set; }
        public string Type { get; set; } = ""; // "checkin" or "checkout"
        public DateTime Time { get; set; }
        public string? Name { get; set; } // optional for checkin
    }
}
