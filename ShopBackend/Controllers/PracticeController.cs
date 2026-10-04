using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PracticeController : ControllerBase
    {
        private readonly IPracticeService _practiceService;

        public PracticeController(IPracticeService practiceService)
        {
            _practiceService = practiceService;
        }

        [HttpGet("current")]
        [ProducesResponseType(typeof(BaseResponse<PracticeResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrent()
        {
            var result = await _practiceService.GetCurrentAsync(GetUserId());
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("current")]
        [ProducesResponseType(typeof(BaseResponse<PracticeResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateCurrent([FromBody] PracticeRequest request)
        {
            var result = await _practiceService.UpdateCurrentAsync(GetUserId(), request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}
