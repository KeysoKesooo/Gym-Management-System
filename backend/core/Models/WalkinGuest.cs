namespace GymManagement.Core.Models.WalkinModel
{
    public class WalkinGuest
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }

    public class WalkinQueueModel
    {
        public int GuestId { get; set; }        // Needed for checkout/update
        public string Type { get; set; } = "";  // "checkin" or "checkout"
        public DateTime Time { get; set; }      // Check-in or Check-out time
    }
}
