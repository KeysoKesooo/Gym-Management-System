using GymManagement.Core.DTOs.UserDto;

namespace GymManagement.Core.DTOs.AuthDto
{
    // DTO for registering a new user
    public class RegisterRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    // DTO for logging in
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }


    public class AuthResponseDto
    {
        public string Token { get; set; } = default!;
        public UserResponseDto User { get; set; } = default!;
    }

}