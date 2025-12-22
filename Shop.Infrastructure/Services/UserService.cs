using Microsoft.Extensions.Logging;
using Shop.Common;
using Shop.Common.Constants;
using Shop.Common.Helpers;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;

namespace Shop.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditService _auditService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            IAuditService auditService,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _auditService = auditService;
            _logger = logger;
        }

        public async Task<BaseResponse<UserProfileResponse>> GetProfileAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return BaseResponse<UserProfileResponse>.ErrorResponse("User not found");
                }

                var response = new UserProfileResponse
                {
                    Id = user.Id,
                    PhoneNumber = user.PhoneNumber,
                    IsTwoFactorEnabled = user.IsTwoFactorEnabled,
                    IsActive = user.Active,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                };

                return BaseResponse<UserProfileResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return BaseResponse<UserProfileResponse>.ErrorResponse(
                    "An error occurred while retrieving profile",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<BaseResponse<UserProfileResponse>> UpdatePhoneNumberAsync(
            Guid userId, ChangePhoneRequest request)
        {
            try
            {
                if (!PhoneNumberValidator.IsValid(request.NewPhoneNumber))
                {
                    return BaseResponse<UserProfileResponse>.ErrorResponse("Invalid phone number format");
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return BaseResponse<UserProfileResponse>.ErrorResponse("User not found");
                }

                if (!PasswordHasher.VerifyPin(request.Pin, user.PinHash))
                {
                    return BaseResponse<UserProfileResponse>.ErrorResponse("Invalid PIN");
                }

                // Check if new phone number is already in use
                if (await _unitOfWork.Users.PhoneNumberExistsAsync(request?.NewPhoneNumber))
                {
                    return BaseResponse<UserProfileResponse>.ErrorResponse(
                        "Phone number already in use"
                    );
                }

                var oldPhoneNumber = user.PhoneNumber;
                user.PhoneNumber = request.NewPhoneNumber;
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                await _auditService.LogActionAsync(
                    userId,
                    "PhoneNumberChanged",
                    "User",
                    userId.ToString(),
                    oldPhoneNumber,
                    request.NewPhoneNumber
                );

                var response = new UserProfileResponse
                {
                    Id = user.Id,
                    PhoneNumber = user.PhoneNumber,
                    IsTwoFactorEnabled = user.IsTwoFactorEnabled,
                    IsActive = user.Active,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt
                };

                return BaseResponse<UserProfileResponse>.SuccessResponse(
                    response,
                    "Phone number updated successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating phone number");
                return BaseResponse<UserProfileResponse>.ErrorResponse(
                    "An error occurred while updating phone number",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<BaseResponse<bool>> ChangePinAsync(Guid userId, ChangePinRequest request)
        {
            try
            {
                if (!PinValidator.IsValid(request.NewPin, AppConstants.PinLength))
                {
                    return BaseResponse<bool>.ErrorResponse(
                        $"Invalid PIN. Must be {AppConstants.PinLength} digits"
                    );
                }

                if (request.NewPin != request.ConfirmNewPin)
                {
                    return BaseResponse<bool>.ErrorResponse("New PIN confirmation does not match");
                }

                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return BaseResponse<bool>.ErrorResponse("User not found");
                }

                if (!PasswordHasher.VerifyPin(request.CurrentPin, user.PinHash))
                {
                    return BaseResponse<bool>.ErrorResponse("Current PIN is incorrect");
                }

                user.PinHash = PasswordHasher.HashPin(request.NewPin);
                user.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                await _auditService.LogActionAsync(
                    userId,
                    "PinChanged",
                    "User",
                    userId.ToString()
                );

                return BaseResponse<bool>.SuccessResponse(true, "PIN changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing PIN");
                return BaseResponse<bool>.ErrorResponse(
                    "An error occurred while changing PIN",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<BaseResponse<bool>> DeactivateAccountAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return BaseResponse<bool>.ErrorResponse("User not found");
                }

                await _unitOfWork.BeginTransactionAsync();

                user.Active = false;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);

                // Revoke all refresh tokens
                await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId);

                await _unitOfWork.CommitTransactionAsync();

                await _auditService.LogActionAsync(
                    userId,
                    "AccountDeactivated",
                    "User",
                    userId.ToString()
                );

                return BaseResponse<bool>.SuccessResponse(true, "Account deactivated successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deactivating account");
                return BaseResponse<bool>.ErrorResponse(
                    "An error occurred while deactivating account",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<BaseResponse<SecurityStatusResponse>> GetSecurityStatusAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return BaseResponse<SecurityStatusResponse>.ErrorResponse("User not found");
                }

                // Count active sessions (valid refresh tokens)
                var activeSessions = await _unitOfWork.RefreshTokens.GetActiveTokenCountAsync(userId);

                var response = new SecurityStatusResponse
                {
                    IsTwoFactorEnabled = user.IsTwoFactorEnabled,
                    ActiveSessions = activeSessions,
                    LastLoginAt = user.LastLoginAt,
                    LastPasswordChange = user.UpdatedAt
                };

                return BaseResponse<SecurityStatusResponse>.SuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving security status");
                return BaseResponse<SecurityStatusResponse>.ErrorResponse(
                    "An error occurred while retrieving security status",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
