using GymManagement.Core.DTOs.UserDto;
using GymManagement.Core.Models.UserModel;
using GymManagement.Core.Repositories.IntUserRepository;
using BCrypt.Net; // For password hashing

namespace GymManagement.Core.Services.IntUserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // 🔹 Get all users
        public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => MapToDto(u));
        }

        // 🔹 Get user by ID
        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        // 🔹 Manual user creation (any role)
        public async Task<UserResponseDto> CreateAsync(UserCreateDto dto)
        {
            var existing = await _userRepository.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new Exception("Email already exists");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = string.IsNullOrEmpty(dto.Role) ? "member" : dto.Role,
                CreatedAt = DateTime.UtcNow // automatically set creation date
            };

            var created = await _userRepository.AddAsync(user);
            return MapToDto(created);
        }

        // 🔹 Update user
        public async Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            user.Name = dto.Name ?? user.Name;
            user.Email = dto.Email ?? user.Email;
            user.Role = dto.Role ?? user.Role;

            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var updated = await _userRepository.UpdateAsync(user);
            return updated == null ? null : MapToDto(updated);
        }

        // 🔹 Delete user
        public async Task<bool> DeleteAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        // 🔹 Helper: Map User entity to UserResponseDto
        private UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }
    }
}