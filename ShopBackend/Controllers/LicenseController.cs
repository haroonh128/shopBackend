using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LicenseController : ControllerBase
    {
        private readonly ILicenseService _licenseService;

        public LicenseController(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        /// <summary>Get all licenses</summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<LicenseResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _licenseService.GetAllAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get license by id</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<LicenseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _licenseService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Create a license</summary>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<LicenseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateLicenseRequest request)
        {
            var result = await _licenseService.CreateAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Update a license</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<LicenseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLicenseRequest request)
        {
            var result = await _licenseService.UpdateAsync(id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Soft-delete a license</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _licenseService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Get subscriptions for a license</summary>
        [HttpGet("{licenseId:guid}/subscriptions")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<LicenseSubscriptionResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubscriptions(Guid licenseId)
        {
            var result = await _licenseService.GetSubscriptionsByLicenseIdAsync(licenseId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Create a license subscription</summary>
        [HttpPost("subscriptions")]
        [ProducesResponseType(typeof(BaseResponse<LicenseSubscriptionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateLicenseSubscriptionRequest request)
        {
            var result = await _licenseService.CreateSubscriptionAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Update a license subscription</summary>
        [HttpPut("subscriptions/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<LicenseSubscriptionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSubscription(Guid id, [FromBody] UpdateLicenseSubscriptionRequest request)
        {
            var result = await _licenseService.UpdateSubscriptionAsync(id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Soft-delete a license subscription</summary>
        [HttpDelete("subscriptions/{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSubscription(Guid id)
        {
            var result = await _licenseService.DeleteSubscriptionAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
