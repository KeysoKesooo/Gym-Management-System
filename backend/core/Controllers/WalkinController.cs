using Microsoft.AspNetCore.Mvc;
using GymManagement.Core.Models.WalkinModel;
using GymManagement.Core.Services.IntWalkinService;

namespace GymManagement.Controllers.WalkinController
{
    [ApiController]
    [Route("api/[controller]")]
    public class WalkinController : ControllerBase
    {
        private readonly IWalkinService _service;

        public WalkinController(IWalkinService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var guests = await _service.GetAllAsync();
            return Ok(guests);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var guest = await _service.GetByIdAsync(id);
            if (guest == null)
                return NotFound(new { message = "Guest not found" });
            return Ok(guest);
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn([FromBody] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new { message = "Name is required" });

            var guest = await _service.CheckInAsync(name);
            return Ok(guest);
        }

        [HttpPost("checkout/{id}")]
        public async Task<IActionResult> CheckOut(int id)
        {
            var guest = await _service.CheckOutAsync(id);
            return Ok(guest);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
