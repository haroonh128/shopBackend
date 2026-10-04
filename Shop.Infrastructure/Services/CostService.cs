using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class CostService : ICostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CostService> _logger;

        public CostService(IUnitOfWork unitOfWork, ILogger<CostService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<CostResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var cost = await _unitOfWork.Costs.GetByIdAsync(id);
                if (cost == null || cost.IsDeleted)
                    return BaseResponse<CostResponse>.ErrorResponse("Cost not found");
                return BaseResponse<CostResponse>.SuccessResponse(MapToResponse(cost));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cost {Id}", id);
                return BaseResponse<CostResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<CostResponse>>> GetAllAsync()
        {
            try
            {
                var list = (await _unitOfWork.Costs.GetAllAsync())
                    .Where(c => !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<CostResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting costs");
                return BaseResponse<IEnumerable<CostResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<CostResponse>>> GetByClientIdAsync(Guid clientId)
        {
            try
            {
                var list = (await _unitOfWork.Costs.GetByClientIdAsync(clientId))
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<CostResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting costs for client {ClientId}", clientId);
                return BaseResponse<IEnumerable<CostResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<CostResponse>> CreateAsync(CreateCostRequest request)
        {
            try
            {
                var error = ValidateAmounts(request.TotalCost, request.Advance);
                if (error != null)
                    return BaseResponse<CostResponse>.ErrorResponse(error);

                var cost = new Cost();
                ApplyRequest(cost, request);
                var added = await _unitOfWork.Costs.AddAsync(cost);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<CostResponse>.SuccessResponse(MapToResponse(added), "Cost created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cost");
                return BaseResponse<CostResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<CostResponse>> UpdateAsync(Guid id, UpdateCostRequest request)
        {
            try
            {
                var cost = await _unitOfWork.Costs.GetByIdAsync(id);
                if (cost == null || cost.IsDeleted)
                    return BaseResponse<CostResponse>.ErrorResponse("Cost not found");

                var error = ValidateAmounts(request.TotalCost, request.Advance);
                if (error != null)
                    return BaseResponse<CostResponse>.ErrorResponse(error);

                ApplyRequest(cost, request);
                _unitOfWork.Costs.Update(cost);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<CostResponse>.SuccessResponse(MapToResponse(cost), "Cost updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cost {Id}", id);
                return BaseResponse<CostResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var cost = await _unitOfWork.Costs.GetByIdAsync(id);
                if (cost == null || cost.IsDeleted)
                    return BaseResponse<bool>.ErrorResponse("Cost not found");

                cost.IsDeleted = true;
                cost.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Costs.Update(cost);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Cost deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cost {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private static string? ValidateAmounts(decimal totalCost, decimal advance)
        {
            if (totalCost < 0)
                return "Total cost cannot be negative";
            if (advance < 0)
                return "Advance cannot be negative";
            if (advance > totalCost)
                return "Advance cannot exceed total cost";
            return null;
        }

        private static void ApplyRequest(Cost cost, CreateCostRequest request)
        {
            cost.ClientId = request.ClientId;
            cost.MeasurementId = request.MeasurementId;
            cost.TotalCost = request.TotalCost;
            cost.Advance = request.Advance;
            cost.Notes = request.Notes;
        }

        private static CostResponse MapToResponse(Cost cost) => new()
        {
            Id = cost.Id,
            ClientId = cost.ClientId,
            MeasurementId = cost.MeasurementId,
            TotalCost = cost.TotalCost,
            Advance = cost.Advance,
            Pending = cost.TotalCost - cost.Advance,
            Notes = cost.Notes,
            CreatedAt = cost.CreatedAt,
            UpdatedAt = cost.UpdatedAt
        };
    }
}
