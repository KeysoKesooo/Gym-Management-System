using GymManagement.Core.DTOs.AuthDto;

namespace GymManagement.Core.Services.IntAuthService
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequest dto);  // Public member registration
        Task<AuthResponseDto> LoginAsync(LoginRequest dto);
    }
}
