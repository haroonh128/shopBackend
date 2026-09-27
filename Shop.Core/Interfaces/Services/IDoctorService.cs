using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IDoctorService
    {
        Task<BaseResponse<IEnumerable<DoctorResponse>>> GetAllAsync(Guid userId);
        Task<BaseResponse<DoctorResponse>> GetByIdAsync(Guid userId, Guid id);
        Task<BaseResponse<DoctorResponse>> CreateAsync(Guid userId, DoctorRequest request);
        Task<BaseResponse<DoctorResponse>> UpdateAsync(Guid userId, Guid id, DoctorRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid userId, Guid id);
    }
}
