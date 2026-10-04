using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IStoreItemService
    {
        Task<BaseResponse<StoreItemResponse>> GetByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<StoreItemResponse>>> GetAllAsync();
        Task<BaseResponse<IEnumerable<StoreItemResponse>>> GetByShopIdAsync(Guid shopId);
        Task<BaseResponse<StoreItemResponse>> CreateAsync(CreateStoreItemRequest request);
        Task<BaseResponse<StoreItemResponse>> UpdateAsync(Guid id, UpdateStoreItemRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
    }
}
