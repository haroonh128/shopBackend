using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IPatientService
    {
        Task<BaseResponse<PagedResult<PatientResponse>>> SearchAsync(Guid userId, PatientSearchRequest request);
        Task<BaseResponse<PatientResponse>> GetByIdAsync(Guid userId, Guid id);
        Task<BaseResponse<PatientResponse>> CreateAsync(Guid userId, PatientRequest request);
        Task<BaseResponse<PatientResponse>> UpdateAsync(Guid userId, Guid id, PatientRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid userId, Guid id);
    }
}
