using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;

        public AdminUserController(IAdminUserService adminUserService)
        {
            _adminUserService = adminUserService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<AdminUserResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] string? search = null)
        {
            var result = await _adminUserService.GetAllAsync(search);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<AdminUserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _adminUserService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<AdminUserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] AdminCreateUserRequest request)
        {
            var result = await _adminUserService.CreateAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<AdminUserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] AdminUpdateUserRequest request)
        {
            var result = await _adminUserService.UpdateAsync(id, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _adminUserService.DeleteAsync(id, GetUserId());
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/active")]
        [ProducesResponseType(typeof(BaseResponse<AdminUserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SetActive(Guid id, [FromBody] SetUserActiveRequest request)
        {
            var result = await _adminUserService.SetActiveAsync(id, request.IsActive);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id:guid}/subscription")]
        [ProducesResponseType(typeof(BaseResponse<AdminUserResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSubscription(Guid id, [FromBody] UpdateSubscriptionRequest request)
        {
            var result = await _adminUserService.UpdateSubscriptionAsync(id, request);
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
