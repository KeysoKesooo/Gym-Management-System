using GymManagement.Core.DTOs.AuthDto;
using GymManagement.Core.DTOs.UserDto;
using GymManagement.Core.Models.UserModel;
using GymManagement.Core.Repositories.IntUserRepository;
using GymManagement.Services.JwtService;
using GymManagement.Core.Services.RedisService;
using BCrypt.Net;

namespace GymManagement.Core.Services.IntAuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwt;
        private readonly RedisSessionService _redis;

        public AuthService(IUserRepository userRepository, JwtService jwt, RedisSessionService redis)
        {
            _userRepository = userRepository;
            _jwt = jwt;
            _redis = redis;
        }

        // 🔹 Register
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequest dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new Exception("Email already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "member",
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            // ✅ FIXED: call async version
            var token = await _jwt.GenerateTokenAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        // 🔹 Login
        public async Task<AuthResponseDto> LoginAsync(LoginRequest dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            // ✅ FIXED: call async version
            var token = await _jwt.GenerateTokenAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt
                }
            };
        }
        // 🔹 Logout
        public async Task LogoutAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    throw new Exception("Token is required for logout");

                // ✅ Remove session from Redis
                await _redis.RemoveSessionAsync(token);
            }
            catch (Exception ex)
            {
                throw new Exception("Error logging out: " + ex.Message, ex);
            }
        }
    }
}