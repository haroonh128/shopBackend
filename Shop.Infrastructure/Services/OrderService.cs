using Microsoft.Extensions.Logging;
using Shop.Common.enums;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<OrderResponse>> GetByIdAsync(Guid userId, Guid id)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(id);
                if (order == null || order.IsDeleted || order.UserId != userId)
                    return BaseResponse<OrderResponse>.ErrorResponse("Order not found");
                return BaseResponse<OrderResponse>.SuccessResponse(MapToResponse(order));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order {Id}", id);
                return BaseResponse<OrderResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<OrderResponse>>> GetAllAsync(Guid userId)
        {
            try
            {
                var list = (await _unitOfWork.Orders.GetByUserIdAsync(userId))
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<OrderResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for user {UserId}", userId);
                return BaseResponse<IEnumerable<OrderResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<OrderResponse>>> GetByClientIdAsync(Guid userId, Guid clientId)
        {
            try
            {
                var list = (await _unitOfWork.Orders.GetByClientIdAsync(userId, clientId))
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<OrderResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for client {ClientId}", clientId);
                return BaseResponse<IEnumerable<OrderResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<OrderResponse>>> GetByStatusAsync(Guid userId, OrderStatus status)
        {
            try
            {
                var list = (await _unitOfWork.Orders.GetByStatusAsync(userId, status))
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<OrderResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders with status {Status}", status);
                return BaseResponse<IEnumerable<OrderResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<OrderResponse>> CreateAsync(Guid userId, CreateOrderRequest request)
        {
            try
            {
                if (request.AdvanceAmount > request.TotalAmount)
                    return BaseResponse<OrderResponse>.ErrorResponse("Advance amount cannot exceed total amount");

                var order = new Orders
                {
                    OrderNumber = await GenerateOrderNumberAsync(),
                    UserId = userId,
                    ClientId = request.ClientId,
                    Type = request.Type,
                    Status = request.Status,
                    Description = request.Description,
                    Quantity = request.Quantity <= 0 ? 1 : request.Quantity,
                    TotalAmount = request.TotalAmount,
                    AdvanceAmount = request.AdvanceAmount,
                    OrderDate = DateTime.UtcNow,
                    DueDate = request.DueDate,
                    DeliveryDate = request.DeliveryDate,
                    Notes = request.Notes
                };

                var added = await _unitOfWork.Orders.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<OrderResponse>.SuccessResponse(MapToResponse(added), "Order created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return BaseResponse<OrderResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<OrderResponse>> UpdateAsync(Guid userId, Guid id, UpdateOrderRequest request)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(id);
                if (order == null || order.IsDeleted || order.UserId != userId)
                    return BaseResponse<OrderResponse>.ErrorResponse("Order not found");

                if (request.AdvanceAmount > request.TotalAmount)
                    return BaseResponse<OrderResponse>.ErrorResponse("Advance amount cannot exceed total amount");

                order.Type = request.Type;
                order.Status = request.Status;
                order.Description = request.Description;
                order.Quantity = request.Quantity <= 0 ? 1 : request.Quantity;
                order.TotalAmount = request.TotalAmount;
                order.AdvanceAmount = request.AdvanceAmount;
                order.DueDate = request.DueDate;
                order.DeliveryDate = request.DeliveryDate;
                order.Notes = request.Notes;

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<OrderResponse>.SuccessResponse(MapToResponse(order), "Order updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {Id}", id);
                return BaseResponse<OrderResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<OrderResponse>> UpdateStatusAsync(Guid userId, Guid id, UpdateOrderStatusRequest request)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(id);
                if (order == null || order.IsDeleted || order.UserId != userId)
                    return BaseResponse<OrderResponse>.ErrorResponse("Order not found");

                order.Status = request.Status;
                if (request.Status == OrderStatus.Delivered && order.DeliveryDate == null)
                    order.DeliveryDate = DateTime.UtcNow;

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<OrderResponse>.SuccessResponse(MapToResponse(order), "Order status updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for order {Id}", id);
                return BaseResponse<OrderResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid userId, Guid id)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(id);
                if (order == null || order.IsDeleted || order.UserId != userId)
                    return BaseResponse<bool>.ErrorResponse("Order not found");

                order.IsDeleted = true;
                order.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Order deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private async Task<string> GenerateOrderNumberAsync()
        {
            string orderNumber;
            do
            {
                orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
            }
            while (await _unitOfWork.Orders.GetByOrderNumberAsync(orderNumber) != null);

            return orderNumber;
        }

        private static OrderResponse MapToResponse(Orders o) => new()
        {
            Id = o.Id,
            OrderNumber = o.OrderNumber,
            UserId = o.UserId,
            ClientId = o.ClientId,
            Type = o.Type,
            Status = o.Status,
            Description = o.Description,
            Quantity = o.Quantity,
            TotalAmount = o.TotalAmount,
            AdvanceAmount = o.AdvanceAmount,
            BalanceAmount = o.TotalAmount - o.AdvanceAmount,
            OrderDate = o.OrderDate,
            DueDate = o.DueDate,
            DeliveryDate = o.DeliveryDate,
            Notes = o.Notes,
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        };
    }
}
