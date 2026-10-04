using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MeasurementController : ControllerBase
    {
        private readonly IMeasurementService _measurementService;

        public MeasurementController(IMeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        /// <summary>Get all measurements</summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<MeasurementResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _measurementService.GetAllAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get measurement by id</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<MeasurementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _measurementService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Get all measurements for a client</summary>
        [HttpGet("client/{clientId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<MeasurementResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByClientId(Guid clientId)
        {
            var result = await _measurementService.GetByClientIdAsync(clientId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Create a measurement</summary>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<MeasurementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMeasurementRequest request)
        {
            var result = await _measurementService.CreateAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Update a measurement</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<MeasurementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMeasurementRequest request)
        {
            var result = await _measurementService.UpdateAsync(id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Soft-delete a measurement</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _measurementService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
