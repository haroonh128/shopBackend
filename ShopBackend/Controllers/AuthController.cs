using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Shop.Core.DTOs;
using Shop.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace ShopBackend.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Login with phone number and PIN
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        /// <summary>
        /// Verify OTP for two-factor authentication
        /// </summary>
        [HttpPost("verify-otp")]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var result = await _authService.VerifyOtpAsync(request);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        /// <summary>
        /// Resend OTP code
        /// </summary>
        [HttpPost("resend-otp")]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var loginRequest = new LoginRequest
            {
                PhoneNumber = request.PhoneNumber,
                Pin = string.Empty // Will be validated server-side
            };

            var result = await _authService.LoginAsync(loginRequest);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AuthResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        /// <summary>
        /// Toggle two-factor authentication
        /// </summary>
        [Authorize]
        [HttpPost("toggle-2fa")]
        [ProducesResponseType(typeof(BaseResponse<TwoFactorStatusResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<TwoFactorStatusResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ToggleTwoFactor([FromBody] TwoFactorToggleRequest request)
        {
            var userId = GetUserId();
            var result = await _authService.ToggleTwoFactorAsync(userId, request.Enable);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Logout and revoke all refresh tokens
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var userId = GetUserId();
            var result = await _authService.LogoutAsync(userId);

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
