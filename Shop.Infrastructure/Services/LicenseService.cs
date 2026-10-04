using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class LicenseService : ILicenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<LicenseService> _logger;

        public LicenseService(IUnitOfWork unitOfWork, ILogger<LicenseService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<LicenseResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var license = await _unitOfWork.Licenses.GetByIdAsync(id);
                if (license == null)
                    return BaseResponse<LicenseResponse>.ErrorResponse("License not found");

                return BaseResponse<LicenseResponse>.SuccessResponse(MapToLicenseResponse(license));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting license {Id}", id);
                return BaseResponse<LicenseResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<LicenseResponse>>> GetAllAsync()
        {
            try
            {
                var licenses = await _unitOfWork.Licenses.GetAllAsync();
                var list = licenses.Select(MapToLicenseResponse).ToList();
                return BaseResponse<IEnumerable<LicenseResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting licenses");
                return BaseResponse<IEnumerable<LicenseResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<LicenseResponse>> CreateAsync(CreateLicenseRequest request)
        {
            try
            {
                if (await _unitOfWork.Licenses.GetByKeyAsync(request.Key) != null)
                    return BaseResponse<LicenseResponse>.ErrorResponse("License key already exists");

                var license = new License
                {
                    Key = request.Key,
                    ProductId = request.ProductId,
                    UserId = request.UserId,
                    IsActive = request.IsActive,
                    CreatedDate = DateTime.UtcNow
                };
                var added = await _unitOfWork.Licenses.AddAsync(license);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<LicenseResponse>.SuccessResponse(MapToLicenseResponse(added), "License created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating license");
                return BaseResponse<LicenseResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<LicenseResponse>> UpdateAsync(Guid id, UpdateLicenseRequest request)
        {
            try
            {
                var license = await _unitOfWork.Licenses.GetByIdAsync(id);
                if (license == null)
                    return BaseResponse<LicenseResponse>.ErrorResponse("License not found");

                license.IsActive = request.IsActive;
                license.ModifiedDate = DateTime.UtcNow;
                _unitOfWork.Licenses.Update(license);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<LicenseResponse>.SuccessResponse(MapToLicenseResponse(license), "License updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating license {Id}", id);
                return BaseResponse<LicenseResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var license = await _unitOfWork.Licenses.GetByIdAsync(id);
                if (license == null)
                    return BaseResponse<bool>.ErrorResponse("License not found");

                license.IsDeleted = true;
                license.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Licenses.Update(license);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "License deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting license {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<LicenseSubscriptionResponse>>> GetSubscriptionsByLicenseIdAsync(Guid licenseId)
        {
            try
            {
                var subs = await _unitOfWork.LicenseSubscriptions.GetByLicenseIdAsync(licenseId);
                var list = subs.Select(MapToSubscriptionResponse).ToList();
                return BaseResponse<IEnumerable<LicenseSubscriptionResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscriptions for license {LicenseId}", licenseId);
                return BaseResponse<IEnumerable<LicenseSubscriptionResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<LicenseSubscriptionResponse>> CreateSubscriptionAsync(CreateLicenseSubscriptionRequest request)
        {
            try
            {
                var license = await _unitOfWork.Licenses.GetByIdAsync(request.LicenseId);
                if (license == null)
                    return BaseResponse<LicenseSubscriptionResponse>.ErrorResponse("License not found");

                var sub = new LicenseSubscription
                {
                    LicenseId = request.LicenseId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Description = request.Description,
                    PackageId = request.PackageId,
                    Status = SubscriptionStatus.Active
                };
                var added = await _unitOfWork.LicenseSubscriptions.AddAsync(sub);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<LicenseSubscriptionResponse>.SuccessResponse(MapToSubscriptionResponse(added), "Subscription created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription");
                return BaseResponse<LicenseSubscriptionResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<LicenseSubscriptionResponse>> UpdateSubscriptionAsync(Guid id, UpdateLicenseSubscriptionRequest request)
        {
            try
            {
                var sub = await _unitOfWork.LicenseSubscriptions.GetByIdAsync(id);
                if (sub == null)
                    return BaseResponse<LicenseSubscriptionResponse>.ErrorResponse("Subscription not found");

                if (request.EndDate.HasValue) sub.EndDate = request.EndDate.Value;
                if (request.Description != null) sub.Description = request.Description;
                if (request.Status.HasValue) sub.Status = request.Status.Value;
                _unitOfWork.LicenseSubscriptions.Update(sub);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<LicenseSubscriptionResponse>.SuccessResponse(MapToSubscriptionResponse(sub), "Subscription updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription {Id}", id);
                return BaseResponse<LicenseSubscriptionResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteSubscriptionAsync(Guid id)
        {
            try
            {
                var sub = await _unitOfWork.LicenseSubscriptions.GetByIdAsync(id);
                if (sub == null)
                    return BaseResponse<bool>.ErrorResponse("Subscription not found");

                sub.IsDeleted = true;
                sub.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.LicenseSubscriptions.Update(sub);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Subscription deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subscription {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private static LicenseResponse MapToLicenseResponse(License l) => new()
        {
            Id = l.Id,
            Key = l.Key,
            ProductId = l.ProductId,
            UserId = l.UserId,
            CreatedDate = l.CreatedDate,
            ModifiedDate = l.ModifiedDate,
            IsActive = l.IsActive,
            CreatedAt = l.CreatedAt
        };

        private static LicenseSubscriptionResponse MapToSubscriptionResponse(LicenseSubscription s) => new()
        {
            Id = s.Id,
            LicenseId = s.LicenseId,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            Description = s.Description,
            Status = s.Status,
            PackageId = s.PackageId,
            CreatedAt = s.CreatedAt
        };
    }
}
