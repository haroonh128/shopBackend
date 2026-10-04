using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IMeasurementService
    {
        Task<BaseResponse<MeasurementResponse>> GetByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<MeasurementResponse>>> GetAllAsync();
        Task<BaseResponse<IEnumerable<MeasurementResponse>>> GetByClientIdAsync(Guid clientId);
        Task<BaseResponse<MeasurementResponse>> CreateAsync(CreateMeasurementRequest request);
        Task<BaseResponse<MeasurementResponse>> UpdateAsync(Guid id, UpdateMeasurementRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
    }
}
