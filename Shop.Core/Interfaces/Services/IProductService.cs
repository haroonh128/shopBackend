using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<BaseResponse<ProductResponse>> GetByIdAsync(Guid id);
        Task<BaseResponse<IEnumerable<ProductResponse>>> GetAllAsync();
        Task<BaseResponse<ProductResponse>> CreateAsync(CreateProductRequest request);
        Task<BaseResponse<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid id);
    }
}
