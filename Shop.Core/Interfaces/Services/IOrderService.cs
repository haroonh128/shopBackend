using Shop.Common.enums;
using Shop.Core.DTOs;

namespace Shop.Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<BaseResponse<OrderResponse>> GetByIdAsync(Guid userId, Guid id);
        Task<BaseResponse<IEnumerable<OrderResponse>>> GetAllAsync(Guid userId);
        Task<BaseResponse<IEnumerable<OrderResponse>>> GetByClientIdAsync(Guid userId, Guid clientId);
        Task<BaseResponse<IEnumerable<OrderResponse>>> GetByStatusAsync(Guid userId, OrderStatus status);
        Task<BaseResponse<OrderResponse>> CreateAsync(Guid userId, CreateOrderRequest request);
        Task<BaseResponse<OrderResponse>> UpdateAsync(Guid userId, Guid id, UpdateOrderRequest request);
        Task<BaseResponse<OrderResponse>> UpdateStatusAsync(Guid userId, Guid id, UpdateOrderStatusRequest request);
        Task<BaseResponse<bool>> DeleteAsync(Guid userId, Guid id);
    }
}
