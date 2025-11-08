using System.ComponentModel.DataAnnotations;

namespace GymManagement.Core.Models.UserModel
{
    public class User
    {
        public int Id { get; set; }

        [Required] public required string Name { get; set; }
        [Required] public required string Email { get; set; }
        [Required] public required string PasswordHash { get; set; }

        [Required] public required string Role { get; set; } = "member"; // admin, staff, member

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
