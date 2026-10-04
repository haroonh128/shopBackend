using Microsoft.Extensions.Logging;
using Shop.Common;
using Shop.Common.Constants;
using Shop.Common.Helpers;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AdminUserService> _logger;

        public AdminUserService(IUnitOfWork unitOfWork, ILogger<AdminUserService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<IEnumerable<AdminUserResponse>>> GetAllAsync(string? search = null)
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllRegisteredUsersAsync(search);
                var response = users.Select(MapToResponse).ToList();
                return BaseResponse<IEnumerable<AdminUserResponse>>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing users for admin");
                return BaseResponse<IEnumerable<AdminUserResponse>>.ErrorResponse(
                    "An error occurred while listing users",
                    new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<AdminUserResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return BaseResponse<AdminUserResponse>.ErrorResponse("User not found");

                return BaseResponse<AdminUserResponse>.SuccessResponse(MapToResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserId}", id);
                return BaseResponse<AdminUserResponse>.ErrorResponse(
                    "An error occurred while retrieving the user",
                    new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<AdminUserResponse>> CreateAsync(AdminCreateUserRequest request)
        {
            try
            {
                var errors = ValidateCreate(request);
                if (errors.Count > 0)
                    return BaseResponse<AdminUserResponse>.ErrorResponse("Validation failed", errors);

                if (await _unitOfWork.Users.PhoneNumberExistsAsync(request.PhoneNumber))
                {
                    return BaseResponse<AdminUserResponse>.ErrorResponse(
                        "User already exists",
                        new List<string> { "An account with this phone number already exists" });
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = request.PhoneNumber.Trim(),
                    PinHash = PasswordHasher.HashPin(request.Pin),
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    Email = request.Email.Trim(),
                    CNIC = request.CNIC.Trim(),
                    AppType = request.AppType?.Trim() ?? string.Empty,
                    ModuleId = request.ModuleId,
                    IsTwoFactorEnabled = request.IsTwoFactorEnabled,
                    IsAdmin = request.IsAdmin,
                    Active = request.IsActive,
                    SubscriptionValidUntil = NormalizeValidityDate(request.SubscriptionValidUntil),
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return BaseResponse<AdminUserResponse>.SuccessResponse(
                    MapToResponse(user),
                    "User created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return BaseResponse<AdminUserResponse>.ErrorResponse(
                    "An error occurred while creating the user",
                    new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<AdminUserResponse>> UpdateAsync(Guid id, AdminUpdateUserRequest request)
        {
            try
            {
                var errors = ValidateUpdate(request);
                if (errors.Count > 0)
                    return BaseResponse<AdminUserResponse>.ErrorResponse("Validation failed", errors);

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return BaseResponse<AdminUserResponse>.ErrorResponse("User not found");

                if (await _unitOfWork.Users.PhoneNumberExistsAsync(request.PhoneNumber, id))
                {
                    return BaseResponse<AdminUserResponse>.ErrorResponse(
                        "Phone number already in use",
                        new List<string> { "Another account already uses this phone number" });
                }

                user.PhoneNumber = request.PhoneNumber.Trim();
                user.FirstName = request.FirstName.Trim();
                user.LastName = request.LastName.Trim();
                user.Email = request.Email.Trim();
                user.CNIC = request.CNIC.Trim();
                user.AppType = request.AppType?.Trim() ?? string.Empty;
                user.ModuleId = request.ModuleId;
                user.IsTwoFactorEnabled = request.IsTwoFactorEnabled;
                user.IsAdmin = request.IsAdmin;
                user.Active = request.IsActive;
                user.SubscriptionValidUntil = NormalizeValidityDate(request.SubscriptionValidUntil);
                user.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(request.Pin))
                {
                    user.PinHash = PasswordHasher.HashPin(request.Pin);
                }

                _unitOfWork.Users.Update(user);

                if (!user.Active)
                {
                    await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(user.Id);
                }

                await _unitOfWork.SaveChangesAsync();

                return BaseResponse<AdminUserResponse>.SuccessResponse(
                    MapToResponse(user),
                    "User updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return BaseResponse<AdminUserResponse>.ErrorResponse(
                    "An error occurred while updating the user",
                    new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid id, Guid actingAdminId)
        {
            try
            {
                if (id == actingAdminId)
                    return BaseResponse<bool>.ErrorResponse("You cannot delete your own admin account");

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return BaseResponse<bool>.ErrorResponse("User not found");

                user.IsDeleted = true;
                user.Active = false;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);

                await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(user.Id);
                await _unitOfWork.SaveChangesAsync();

                return BaseResponse<bool>.SuccessResponse(true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return BaseResponse<bool>.ErrorResponse(
                    "An error occurred while deleting the user",
                    new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<AdminUserResponse>> SetActiveAsync(Guid id, bool isActive)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return BaseResponse<AdminUserResponse>.ErrorResponse("User not found");

                user.Active = isActive;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);

                if (!isActive)
                {
                    await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(user.Id);
                }

                await _unitOfWork.SaveChangesAsync();

                return BaseResponse<AdminUserResponse>.SuccessResponse(
                    MapToResponse(user),
                    isActive ? "User activated successfully" : "User inactivated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting active status for user {UserId}", id);
                return BaseResponse<AdminUserResponse>.ErrorResponse(
                    "An error occurred while updating user status",
                    new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<AdminUserResponse>> UpdateSubscriptionAsync(
            Guid id, UpdateSubscriptionRequest request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return BaseResponse<AdminUserResponse>.ErrorResponse("User not found");

                user.SubscriptionValidUntil = NormalizeValidityDate(request.SubscriptionValidUntil);
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return BaseResponse<AdminUserResponse>.SuccessResponse(
                    MapToResponse(user),
                    "Subscription validity updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subscription for user {UserId}", id);
                return BaseResponse<AdminUserResponse>.ErrorResponse(
                    "An error occurred while updating subscription",
                    new List<string> { ex.Message });
            }
        }

        private static List<string> ValidateCreate(AdminCreateUserRequest request)
        {
            var errors = new List<string>();

            if (!PhoneNumberValidator.IsValid(request.PhoneNumber))
                errors.Add("Invalid phone number format. Use format: +1234567890");

            if (!PinValidator.IsValid(request.Pin, AppConstants.PinLength))
                errors.Add($"PIN must be exactly {AppConstants.PinLength} digits");

            if (request.Pin != request.ConfirmPin)
                errors.Add("PIN and Confirm PIN must match");

            if (string.IsNullOrWhiteSpace(request.FirstName))
                errors.Add("First name is required");

            if (string.IsNullOrWhiteSpace(request.LastName))
                errors.Add("Last name is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required");

            return errors;
        }

        private static List<string> ValidateUpdate(AdminUpdateUserRequest request)
        {
            var errors = new List<string>();

            if (!PhoneNumberValidator.IsValid(request.PhoneNumber))
                errors.Add("Invalid phone number format. Use format: +1234567890");

            if (!string.IsNullOrWhiteSpace(request.Pin))
            {
                if (!PinValidator.IsValid(request.Pin, AppConstants.PinLength))
                    errors.Add($"PIN must be exactly {AppConstants.PinLength} digits");

                if (request.Pin != request.ConfirmPin)
                    errors.Add("PIN and Confirm PIN must match");
            }

            if (string.IsNullOrWhiteSpace(request.FirstName))
                errors.Add("First name is required");

            if (string.IsNullOrWhiteSpace(request.LastName))
                errors.Add("Last name is required");

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required");

            return errors;
        }

        private static DateTime? NormalizeValidityDate(DateTime? value)
        {
            if (!value.HasValue) return null;
            return value.Value.Date;
        }

        internal static bool IsSubscriptionExpired(User user)
        {
            if (user.IsAdmin) return false;
            // Null means not yet managed by admin (legacy / until first paid period is set).
            if (!user.SubscriptionValidUntil.HasValue) return false;
            return user.SubscriptionValidUntil.Value.Date < DateTime.UtcNow.Date;
        }

        private static AdminUserResponse MapToResponse(User user)
        {
            return new AdminUserResponse
            {
                Id = user.Id,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CNIC = user.CNIC,
                AppType = user.AppType,
                ModuleId = user.ModuleId,
                IsTwoFactorEnabled = user.IsTwoFactorEnabled,
                IsActive = user.Active,
                IsAdmin = user.IsAdmin,
                SubscriptionValidUntil = user.SubscriptionValidUntil,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsSubscriptionExpired = IsSubscriptionExpired(user)
            };
        }
    }
}
