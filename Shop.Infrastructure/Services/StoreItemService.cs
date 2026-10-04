using Microsoft.Extensions.Logging;
using Shop.Common.enums;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class StoreItemService : IStoreItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StoreItemService> _logger;

        public StoreItemService(IUnitOfWork unitOfWork, ILogger<StoreItemService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<StoreItemResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var item = await _unitOfWork.StoreItems.GetByIdAsync(id);
                if (item == null)
                    return BaseResponse<StoreItemResponse>.ErrorResponse("Store item not found");
                return BaseResponse<StoreItemResponse>.SuccessResponse(MapToResponse(item));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store item {Id}", id);
                return BaseResponse<StoreItemResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<StoreItemResponse>>> GetAllAsync()
        {
            try
            {
                var list = (await _unitOfWork.StoreItems.GetAllAsync()).Select(MapToResponse).ToList();
                return BaseResponse<IEnumerable<StoreItemResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store items");
                return BaseResponse<IEnumerable<StoreItemResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<StoreItemResponse>>> GetByShopIdAsync(Guid shopId)
        {
            try
            {
                var list = (await _unitOfWork.StoreItems.GetByShopIdAsync(shopId)).Select(MapToResponse).ToList();
                return BaseResponse<IEnumerable<StoreItemResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting store items for shop {ShopId}", shopId);
                return BaseResponse<IEnumerable<StoreItemResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<StoreItemResponse>> CreateAsync(CreateStoreItemRequest request)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(request.ShopId);
                if (product == null)
                    return BaseResponse<StoreItemResponse>.ErrorResponse("Shop (Product) not found");

                var item = new StoreItem
                {
                    Name = request.Name,
                    Description = request.Description,
                    Cost = request.Cost,
                    SellingPrice = request.SellingPrice,
                    Quantity = request.Quantity,
                    Unit = (UOM)request.Unit,
                    IsActive = request.IsActive,
                    ShopId = request.ShopId
                };
                var added = await _unitOfWork.StoreItems.AddAsync(item);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<StoreItemResponse>.SuccessResponse(MapToResponse(added), "Store item created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating store item");
                return BaseResponse<StoreItemResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<StoreItemResponse>> UpdateAsync(Guid id, UpdateStoreItemRequest request)
        {
            try
            {
                var item = await _unitOfWork.StoreItems.GetByIdAsync(id);
                if (item == null)
                    return BaseResponse<StoreItemResponse>.ErrorResponse("Store item not found");

                item.Name = request.Name;
                item.Description = request.Description;
                item.Cost = request.Cost;
                item.SellingPrice = request.SellingPrice;
                item.Quantity = request.Quantity;
                item.Unit = (UOM)request.Unit;
                item.IsActive = request.IsActive;
                _unitOfWork.StoreItems.Update(item);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<StoreItemResponse>.SuccessResponse(MapToResponse(item), "Store item updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating store item {Id}", id);
                return BaseResponse<StoreItemResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var item = await _unitOfWork.StoreItems.GetByIdAsync(id);
                if (item == null)
                    return BaseResponse<bool>.ErrorResponse("Store item not found");

                item.IsDeleted = true;
                item.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.StoreItems.Update(item);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Store item deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting store item {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private static StoreItemResponse MapToResponse(StoreItem s) => new()
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Cost = s.Cost,
            SellingPrice = s.SellingPrice,
            Quantity = s.Quantity,
            Unit = (int)s.Unit,
            IsActive = s.IsActive,
            ShopId = s.ShopId,
            CreatedAt = s.CreatedAt
        };
    }
}
