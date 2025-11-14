using GymManagement.Core.DTOs.UserDto;
using System.Security.Claims;

namespace GymManagement.Core.Services.IntUserService
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<UserResponseDto> CreateAsync(UserCreateDto dto, string role); // include role for admin/member logic
        Task<UserResponseDto?> UpdateAsync(int id, UserUpdateDto dto, ClaimsPrincipal user); // include user for rate-limit & role
        Task<bool> DeleteAsync(int id, string role); // include role for admin check
    }
}
