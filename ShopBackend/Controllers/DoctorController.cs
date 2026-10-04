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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<DoctorResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _doctorService.GetAllAsync(GetUserId());
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<DoctorResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _doctorService.GetByIdAsync(GetUserId(), id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<DoctorResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] DoctorRequest request)
        {
            var result = await _doctorService.CreateAsync(GetUserId(), request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<DoctorResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] DoctorRequest request)
        {
            var result = await _doctorService.UpdateAsync(GetUserId(), id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _doctorService.DeleteAsync(GetUserId(), id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}
