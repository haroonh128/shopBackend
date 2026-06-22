using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface ILicenseService
    {
        Task<BaseResponse<LicenseResponse>> GetByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<LicenseResponse>>> GetAllAsync();
        Task<BaseResponse<LicenseResponse>> CreateAsync(CreateLicenseRequest request);
        Task<BaseResponse<LicenseResponse>> UpdateAsync(Guid id, UpdateLicenseRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
        Task<BaseResponse<IEnumerable<LicenseSubscriptionResponse>>> GetSubscriptionsByLicenseIdAsync(Guid licenseId);
        Task<BaseResponse<LicenseSubscriptionResponse>> CreateSubscriptionAsync(CreateLicenseSubscriptionRequest request);
        Task<BaseResponse<LicenseSubscriptionResponse>> UpdateSubscriptionAsync(Guid id, UpdateLicenseSubscriptionRequest request);
        Task<BaseResponse<bool>> DeleteSubscriptionAsync(Guid id);
    }
}
