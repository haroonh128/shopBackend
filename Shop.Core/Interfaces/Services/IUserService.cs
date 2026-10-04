using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<BaseResponse<UserProfileResponse>> GetProfileAsync(Guid userId);
        Task<BaseResponse<UserProfileResponse>> UpdatePhoneNumberAsync(Guid userId, ChangePhoneRequest request);
        Task<BaseResponse<bool>> ChangePinAsync(Guid userId, ChangePinRequest request);
        Task<BaseResponse<bool>> DeactivateAccountAsync(Guid userId);
        Task<BaseResponse<SecurityStatusResponse>> GetSecurityStatusAsync(Guid userId);
    }
}
