using Microsoft.AspNetCore.Mvc;
using GymManagement.Core.DTOs.UserDto;
using GymManagement.Core.DTOs.AuthDto;
using GymManagement.Core.Services.IntUserService;
using GymManagement.Core.Services.IntAuthService;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace GymManagement.Controllers.UsersController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protect all endpoints
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService; // optional

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
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "member"; // current user role
                var response = await _userService.CreateAsync(dto, role);
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
            try
            {
                var users = await _userService.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ GET current user
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            if (HttpContext.Items["UserSession"] is not SessionDto session)
                return Unauthorized(new { error = "Not logged in" });

            var userDto = new UserResponseDto
            {
                Id = session.Id,
                Name = session.Name,
                Email = session.Email,
                Role = session.Role,
            };

            return Ok(userDto);
        }

        // ✅ GET user by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null) return NotFound(new { message = "User not found" });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ UPDATE user
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            try
            {
                var updated = await _userService.UpdateAsync(id, dto, User); // pass ClaimsPrincipal
                if (updated == null) return NotFound(new { message = "User not found" });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ DELETE user
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var role = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
                var deleted = await _userService.DeleteAsync(id, role); // pass role
                if (!deleted) return NotFound(new { message = "User not found or unauthorized" });
                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
