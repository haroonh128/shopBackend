using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CostController : ControllerBase
    {
        private readonly ICostService _costService;

        public CostController(ICostService costService)
        {
            _costService = costService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<CostResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _costService.GetAllAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<CostResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _costService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("client/{clientId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<CostResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByClientId(Guid clientId)
        {
            var result = await _costService.GetByClientIdAsync(clientId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<CostResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCostRequest request)
        {
            var result = await _costService.CreateAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<CostResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCostRequest request)
        {
            var result = await _costService.UpdateAsync(id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _costService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
