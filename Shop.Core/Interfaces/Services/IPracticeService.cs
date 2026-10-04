using Shop.Core.DTOs;
using Shop.Entities;

namespace Shop.Core.Interfaces.Services
{
    public interface IPracticeService
    {
        Task<BaseResponse<PracticeResponse>> GetCurrentAsync(Guid userId);
        Task<BaseResponse<PracticeResponse>> UpdateCurrentAsync(Guid userId, PracticeRequest request);
        Task<Practice> GetOrCreatePracticeEntityAsync(Guid userId);
    }
}
