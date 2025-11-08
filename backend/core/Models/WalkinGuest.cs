namespace GymManagement.Core.Models.WalkinModel
{
    public class WalkinGuest
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
    }
}
