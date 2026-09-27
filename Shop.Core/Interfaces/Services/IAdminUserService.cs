using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IAdminUserService
    {
        Task<BaseResponse<IEnumerable<AdminUserResponse>>> GetAllAsync(string? search = null);
        Task<BaseResponse<AdminUserResponse>> GetByIdAsync(Guid id);
        Task<BaseResponse<AdminUserResponse>> CreateAsync(AdminCreateUserRequest request);
        Task<BaseResponse<AdminUserResponse>> UpdateAsync(Guid id, AdminUpdateUserRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid id, Guid actingAdminId);
        Task<BaseResponse<AdminUserResponse>> SetActiveAsync(Guid id, bool isActive);
        Task<BaseResponse<AdminUserResponse>> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionRequest request);
    }
}
