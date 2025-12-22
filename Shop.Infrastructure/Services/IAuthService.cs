
using Shop.Core.DTOs;

namespace Shop.Infrastructure.Services
{
    public interface IAuthService
    {
        Task<BaseResponse<TwoFactorStatusResponse>> ToggleTwoFactorAsync(Guid userId, bool enable);
        Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<BaseResponse<AuthResponse>> VerifyOtpAsync(VerifyOtpRequest request);
        Task<BaseResponse<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<BaseResponse<AuthResponse>> RefreshTokenAsync(string refreshToken);
        Task<BaseResponse<bool>> LogoutAsync(Guid userId);

    }
}