using System.ComponentModel.DataAnnotations;

namespace GymManagement.Core.DTOs.UserDto
{

    public class UserCreateDto
    {
        [Required, MaxLength(100)]
        public required string Name { get; set; }
        [Required, EmailAddress, MaxLength(150)]
        public required string Email { get; set; }
        [Required, MinLength(8), MaxLength(100)]
        public required string Password { get; set; }
        public string? Role { get; set; }
    }

    public class UserUpdateDto
    {
        public string? Name { get; set; }
        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }
        [MinLength(8), MaxLength(100)]
        public string? Password { get; set; }
        public string? Role { get; set; }
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    public class UserQueueDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "member";
    }


}
