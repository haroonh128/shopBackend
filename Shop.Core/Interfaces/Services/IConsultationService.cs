using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IConsultationService
    {
        Task<BaseResponse<IEnumerable<ConsultationListItemResponse>>> GetByPatientAsync(Guid userId, Guid patientId);
        Task<BaseResponse<ConsultationDetailsResponse>> GetByIdAsync(Guid userId, Guid id);
        Task<BaseResponse<ConsultationDetailsResponse>> CreateAsync(Guid userId, ConsultationRequest request);
        Task<BaseResponse<ConsultationDetailsResponse>> UpdateAsync(Guid userId, Guid id, ConsultationRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid userId, Guid id);
    }
}
