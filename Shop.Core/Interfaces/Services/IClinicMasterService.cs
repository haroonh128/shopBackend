using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IClinicMasterService
    {
        Task<BaseResponse<IEnumerable<DiagnosisResponse>>> SearchDiagnosesAsync(Guid userId, string? query);
        Task<BaseResponse<DiagnosisResponse>> CreateDiagnosisAsync(Guid userId, DiagnosisRequest request);
        Task<BaseResponse<IEnumerable<TestResponse>>> SearchTestsAsync(Guid userId, string? query);
        Task<BaseResponse<TestResponse>> CreateTestAsync(Guid userId, TestRequest request);
        Task<BaseResponse<IEnumerable<MedicationResponse>>> SearchMedicationsAsync(Guid userId, string? query);
        Task<BaseResponse<MedicationResponse>> CreateMedicationAsync(Guid userId, MedicationRequest request);
    }
}
