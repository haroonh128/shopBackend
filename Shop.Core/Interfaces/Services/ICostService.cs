using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface ICostService
    {
        Task<BaseResponse<CostResponse>> GetByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<CostResponse>>> GetAllAsync();
        Task<BaseResponse<IEnumerable<CostResponse>>> GetByClientIdAsync(Guid clientId);
        Task<BaseResponse<CostResponse>> CreateAsync(CreateCostRequest request);
        Task<BaseResponse<CostResponse>> UpdateAsync(Guid id, UpdateCostRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
    }
}
