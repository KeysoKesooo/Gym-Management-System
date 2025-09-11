using System.ComponentModel.DataAnnotations;

namespace GymManagement.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required] public required string Name { get; set; }
        [Required] public required string Email { get; set; }
        [Required] public required string PasswordHash { get; set; }

        [Required] public required string Role { get; set; } // admin, staff, member

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
