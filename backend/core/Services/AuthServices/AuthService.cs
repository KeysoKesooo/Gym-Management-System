using GymManagement.Core.DTOs.AuthDto;
using GymManagement.Core.DTOs.UserDto;
using GymManagement.Core.Models.UserModel;
using GymManagement.Core.Repositories.IntUserRepository;
using GymManagement.Services.JwtService;
using GymManagement.Core.Services.SessionService;
using GymManagement.Core.Services.RatelimiterService;
using GymManagement.Core.Services.CacheService;
using BCrypt.Net;

namespace GymManagement.Core.Services.IntAuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwt;
        private readonly RedisSessionService _redis;
        private readonly RedisRateLimiter _rateLimiter;
        private readonly RedisCacheService _cache;

        private static readonly TimeSpan LoginRateLimitPeriod = TimeSpan.FromMinutes(1);
        private const int LoginRateLimitCount = 5;
        private static readonly TimeSpan CacheTTL = TimeSpan.FromMinutes(1); // TTL for user cache

        public AuthService(
            IUserRepository userRepository,
            JwtService jwt,
            RedisSessionService redis,
            RedisRateLimiter rateLimiter,
            RedisCacheService cache)
        {
            _userRepository = userRepository;
            _jwt = jwt;
            _redis = redis;
            _rateLimiter = rateLimiter;
            _cache = cache;
        }

        private string GetCacheKey(string email) => $"user:{email}";

        // 🔹 Register
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequest dto)
        {
            try
            {
                var rateKey = $"register:{dto.Email}";
                var allowed = await _rateLimiter.IsAllowedAsync(rateKey, LoginRateLimitCount, LoginRateLimitPeriod);
                if (!allowed)
                    throw new Exception("Too many registration attempts. Please wait before trying again.");

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

                var token = await _jwt.GenerateTokenAsync(user);

                // Cache user profile
                await _cache.SetCacheAsync(GetCacheKey(user.Email), user, CacheTTL);

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
            catch (Exception ex)
            {
                throw new Exception($"Error registering user: {ex.Message}", ex);
            }
        }

        // 🔹 Login
        public async Task<AuthResponseDto> LoginAsync(LoginRequest dto)
        {
            try
            {
                var rateKey = $"login:{dto.Email}";
                var allowed = await _rateLimiter.IsAllowedAsync(rateKey, LoginRateLimitCount, LoginRateLimitPeriod);
                if (!allowed)
                    throw new Exception("Too many login attempts. Please wait before trying again.");

                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                    throw new Exception("Email and password are required");

                // Try get user from cache
                var cachedUser = await _cache.GetCacheAsync<User>(GetCacheKey(dto.Email));
                User user = (cachedUser ?? await _userRepository.GetByEmailAsync(dto.Email))
                ?? throw new Exception("Invalid email or password");



                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                    throw new Exception("Invalid email or password");

                var token = await _jwt.GenerateTokenAsync(user);

                // Cache user profile if not cached yet
                if (cachedUser == null)
                    await _cache.SetCacheAsync(GetCacheKey(user.Email), user, CacheTTL);

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
            catch (Exception ex)
            {
                throw new Exception($"Error logging in user: {ex.Message}", ex);
            }
        }

        // 🔹 Logout
        public async Task LogoutAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    throw new Exception("Token is required for logout");

                await _redis.RemoveSessionAsync(token);
            }
            catch (Exception ex)
            {
                throw new Exception("Error logging out: " + ex.Message, ex);
            }
        }
    }
}
