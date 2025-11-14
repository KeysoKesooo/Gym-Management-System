using System.Security.Claims;
using GymManagement.Core.DTOs.UserDto;
using GymManagement.Core.Models.UserModel;
using GymManagement.Core.Repositories.IntUserRepository;
using GymManagement.Core.Services.CacheService;

namespace GymManagement.Core.Services.IntUserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly RedisCacheService _cache;
        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(1); // TTL for cache

        public UserService(IUserRepository userRepository, RedisCacheService cache)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        private string GetCacheKey(int? userId = null) =>
            userId.HasValue ? $"user:{userId}" : "users:all";

        // ------------------------
        // GET ALL USERS
        // ------------------------
        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var cacheKey = GetCacheKey();
            var cached = await _cache.GetCacheAsync<IEnumerable<UserResponseDto>>(cacheKey);
            if (cached != null) return cached;

            var users = await _userRepository.GetAllAsync();
            var dtoList = users.Select(MapToDto).ToList();

            await _cache.SetCacheAsync(cacheKey, dtoList, _ttl);
            return dtoList;
        }

        // ------------------------
        // GET USER BY ID
        // ------------------------
        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var cacheKey = GetCacheKey(id);
            var cached = await _cache.GetCacheAsync<UserResponseDto>(cacheKey);
            if (cached != null) return cached;

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var dto = MapToDto(user);
            await _cache.SetCacheAsync(cacheKey, dto, _ttl);
            return dto;
        }

        // ------------------------
        // CREATE USER (Write-behind)
        // ------------------------
        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto, string role)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = string.IsNullOrEmpty(dto.Role) ? "member" : dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            // Update cache immediately (write-behind)
            var cacheKey = GetCacheKey();
            var cachedList = await _cache.GetCacheAsync<List<UserResponseDto>>(cacheKey) ?? new List<UserResponseDto>();
            var tempDto = MapToDto(user);
            cachedList.Add(tempDto);
            await _cache.SetCacheAsync(cacheKey, cachedList, _ttl);

            // Background save
            _ = Task.Run(async () => await _userRepository.AddAsync(user));

            return tempDto;
        }

        // ------------------------
        // UPDATE USER (Write-behind)
        // ------------------------
        public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto, ClaimsPrincipal user)
        {
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name ?? existing.Name;
            existing.Email = dto.Email ?? existing.Email;
            existing.Role = dto.Role ?? existing.Role;

            if (!string.IsNullOrEmpty(dto.Password))
                existing.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Update cache immediately
            var cacheKey = GetCacheKey(existing.Id);
            await _cache.SetCacheAsync(cacheKey, MapToDto(existing), _ttl);

            // Background save
            _ = Task.Run(async () => await _userRepository.UpdateAsync(existing));

            return MapToDto(existing);
        }

        // ------------------------
        // DELETE USER
        // ------------------------
        public async Task<bool> DeleteAsync(int id, string role)
        {
            if (role != "admin")
                throw new UnauthorizedAccessException("Only admin can delete users.");

            var deleted = await _userRepository.DeleteAsync(id);
            if (deleted)
                await _cache.RemoveCacheAsync(GetCacheKey(id));

            return deleted;
        }

        // ------------------------
        // Helper: Map User to DTO
        // ------------------------
        private UserResponseDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }
}
