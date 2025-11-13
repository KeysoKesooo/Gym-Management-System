using Microsoft.AspNetCore.Mvc;
using GymManagement.Core.DTOs.UserDto;
using GymManagement.Core.Services.IntUserService;
using GymManagement.Core.Services.IntAuthService;
using GymManagement.Core.DTOs.AuthDto;
using Microsoft.AspNetCore.Authorization;
namespace GymManagement.Controllers.UsersController
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService; // optional if needed

        public UserController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        // 🔹 Admin/manual creation (any role)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            try
            {
                var response = await _userService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ GET all users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            // Middleware attaches SessionDto to HttpContext.Items
            if (HttpContext.Items["UserSession"] is not SessionDto session)
                return Unauthorized(new { error = "Not logged in" });

            // Optional: map to UserResponseDto if needed
            var userDto = new UserResponseDto
            {
                Id = session.Id,
                Name = session.Name,
                Email = session.Email,
                Role = session.Role,
                // CreatedAt can be added if SessionDto includes it
            };

            return Ok(userDto);
        }


        // ✅ GET user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });
            return Ok(user);
        }

        // ✅ UPDATE user
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            var updated = await _userService.UpdateAsync(id, dto);
            if (updated == null) return NotFound(new { message = "User not found" });
            return Ok(updated);
        }

        // ✅ DELETE user
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = "User not found" });
            return Ok(new { message = "User deleted successfully" });
        }
    }
}
