using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;
namespace ShopBackend.Controllers
{


    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var result = await _userService.GetProfileAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Update phone number
        /// </summary>
        [HttpPut("phone-number")]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<UserProfileResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePhoneNumber([FromBody] ChangePhoneRequest request)
        {
            var userId = GetUserId();
            var result = await _userService.UpdatePhoneNumberAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Change PIN
        /// </summary>
        [HttpPut("change-pin")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePin([FromBody] ChangePinRequest request)
        {
            var userId = GetUserId();
            var result = await _userService.ChangePinAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Get security status
        /// </summary>
        [HttpGet("security-status")]
        [ProducesResponseType(typeof(BaseResponse<SecurityStatusResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSecurityStatus()
        {
            var userId = GetUserId();
            var result = await _userService.GetSecurityStatusAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Deactivate account
        /// </summary>
        [HttpDelete("deactivate")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeactivateAccount()
        {
            var userId = GetUserId();
            var result = await _userService.DeactivateAccountAsync(userId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}
