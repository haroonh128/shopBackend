using Shop.Common.Constants;
using Shop.Common;
using Shop.Common.Helpers;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Entities;
using Microsoft.Extensions.Logging;

namespace Shop.Infrastructure.Services
{
        

    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ISmsService _smsService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            ISmsService smsService,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _smsService = smsService;
            _logger = logger;
        }

        public async Task<BaseResponse<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Validate phone number
                if (!PhoneNumberValidator.IsValid(request.PhoneNumber))
                {
                    return BaseResponse<AuthResponse>.ErrorResponse(
                        "Invalid phone number format. Use format: +1234567890",
                        new List<string> { "Phone number must start with + and contain 10-15 digits" }
                    );
                }

                // Validate PIN
                if (!PinValidator.IsValid(request.Pin, AppConstants.PinLength))
                {
                    return BaseResponse<AuthResponse>.ErrorResponse(
                        "Invalid PIN",
                        new List<string> { $"PIN must be exactly {AppConstants.PinLength} digits" }
                    );
                }

                // Check PIN confirmation
                if (request.Pin != request.ConfirmPin)
                {
                    return BaseResponse<AuthResponse>.ErrorResponse(
                        "PIN confirmation does not match",
                        new List<string> { "PIN and Confirm PIN must match" }
                    );
                }

                // Check if user already exists
                if (await _unitOfWork.Users.PhoneNumberExistsAsync(request.PhoneNumber))
                {
                    return BaseResponse<AuthResponse>.ErrorResponse(
                        "User already exists",
                        new List<string> { "An account with this phone number already exists" }
                    );
                }

                await _unitOfWork.BeginTransactionAsync();

                // Create user
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    PhoneNumber = request.PhoneNumber,
                    PinHash = PasswordHasher.HashPin(request.Pin),
                    IsTwoFactorEnabled = false,
                    Active = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Generate tokens
                var accessToken = _tokenService.GenerateJwtToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                var refreshTokenEntity = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = refreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays),
                    IsRevoked = false
                };

                await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);
                await _unitOfWork.CommitTransactionAsync();

                var response = new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AppConstants.AccessTokenExpiryMinutes),
                    RequiresTwoFactor = false,
                    User = new UserInfo
                    {
                        Id = user.Id,
                        PhoneNumber = user.PhoneNumber,
                        IsTwoFactorEnabled = user.IsTwoFactorEnabled
                    }
                };

                return BaseResponse<AuthResponse>.SuccessResponse(response, "Registration successful");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error during user registration");
                return BaseResponse<AuthResponse>.ErrorResponse(
                    "An error occurred during registration",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<BaseResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                // Validate inputs
                if (!PhoneNumberValidator.IsValid(request.PhoneNumber))
                {
                    return BaseResponse<AuthResponse>.ErrorResponse("Invalid phone number format");
                }

                if (!PinValidator.IsValid(request.Pin, AppConstants.PinLength))
                {
                    return BaseResponse<AuthResponse>.ErrorResponse("Invalid PIN format");
                }

                // Get user
                var user = await _unitOfWork.Users.GetByPhoneNumberAsync(request.PhoneNumber);

                if (user == null || !PasswordHasher.VerifyPin(request.Pin, user.PinHash))
                {
                    return BaseResponse<AuthResponse>.ErrorResponse("Invalid phone number or PIN");
                }

                if (!user.Active)
                {
                    return BaseResponse<AuthResponse>.ErrorResponse("Account is deactivated");
                }

                // Check if 2FA is enabled
                if (user.IsTwoFactorEnabled)
                {
                    await _unitOfWork.BeginTransactionAsync();

                    // Check active OTP count to prevent spam
                    var activeOtpCount = await _unitOfWork.OtpCodes.GetActiveOtpCountAsync(user.Id);
                    if (activeOtpCount >= AppConstants.MaxOtpAttempts)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return BaseResponse<AuthResponse>.ErrorResponse(
                            "Too many OTP requests. Please wait before trying again."
                        );
                    }

                    // Generate OTP
                    var otpCode = OtpGenerator.GenerateOtp(AppConstants.OtpLength);

                    var otp = new OtpCode
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Code = otpCode,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(AppConstants.OtpExpiryMinutes),
                        IsUsed = false
                    };

                    await _unitOfWork.OtpCodes.AddAsync(otp);
                    await _unitOfWork.SaveChangesAsync();

                    // Send OTP
                    var smsSent = await _smsService.SendOtpAsync(user.PhoneNumber, otpCode);

                    if (!smsSent)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return BaseResponse<AuthResponse>.ErrorResponse(
                            "Failed to send OTP. Please try again."
                        );
                    }

                    await _unitOfWork.CommitTransactionAsync();

                    var response = new AuthResponse
                    {
                        RequiresTwoFactor = true,
                        User = new UserInfo
                        {
                            Id = user.Id,
                            PhoneNumber = user.PhoneNumber,
                            IsTwoFactorEnabled = user.IsTwoFactorEnabled
                        }
                    };

                    return BaseResponse<AuthResponse>.SuccessResponse(
                        response,
                        $"OTP sent to your phone number. Valid for {AppConstants.OtpExpiryMinutes} minutes."
                    );
                }

                // No 2FA - generate tokens directly
                await _unitOfWork.BeginTransactionAsync();

                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                var refreshTokenEntity = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = refreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays),
                    IsRevoked = false
                };

                await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);

                user.LastLoginAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);

                await _unitOfWork.CommitTransactionAsync();

                var loginResponse = new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AppConstants.AccessTokenExpiryMinutes),
                    RequiresTwoFactor = false,
                    User = new UserInfo
                    {
                        Id = user.Id,
                        PhoneNumber = user.PhoneNumber,
                        IsTwoFactorEnabled = user.IsTwoFactorEnabled
                    }
                };

                return BaseResponse<AuthResponse>.SuccessResponse(loginResponse, "Login successful");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error during login");
                return BaseResponse<AuthResponse>.ErrorResponse(
                    "An error occurred during login",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<BaseResponse<AuthResponse>> VerifyOtpAsync(VerifyOtpRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var user = await _unitOfWork.Users.GetByPhoneNumberAsync(request.PhoneNumber);

                if (user == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return BaseResponse<AuthResponse>.ErrorResponse("Invalid request");
                }

                var otp = await _unitOfWork.OtpCodes.GetValidOtpAsync(user.Id, request.OtpCode);

                if (otp == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return BaseResponse<AuthResponse>.ErrorResponse(
                        "Invalid or expired OTP code"
                    );
                }

                // Mark OTP as used
                otp.IsUsed = true;
                otp.UsedAt = DateTime.UtcNow;
                _unitOfWork.OtpCodes.Update(otp);

                // Generate tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                var refreshTokenEntity = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = refreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays),
                    IsRevoked = false
                };

                await _unitOfWork.RefreshTokens.AddAsync(refreshTokenEntity);

                user.LastLoginAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);

                await _unitOfWork.CommitTransactionAsync();

                var response = new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AppConstants.AccessTokenExpiryMinutes),
                    RequiresTwoFactor = false,
                    User = new UserInfo
                    {
                        Id = user.Id,
                        PhoneNumber = user.PhoneNumber,
                        IsTwoFactorEnabled = user.IsTwoFactorEnabled
                    }
                };

                return BaseResponse<AuthResponse>.SuccessResponse(response, "OTP verified successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error during OTP verification");
                return BaseResponse<AuthResponse>.ErrorResponse(
                    "An error occurred during OTP verification",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<BaseResponse<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var tokenEntity = await _unitOfWork.RefreshTokens.GetValidTokenAsync(refreshToken);

                if (tokenEntity == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return BaseResponse<AuthResponse>.ErrorResponse("Invalid or expired refresh token");
                }

                // Revoke old token
                tokenEntity.IsRevoked = true;
                tokenEntity.RevokedAt = DateTime.UtcNow;
                _unitOfWork.RefreshTokens.Update(tokenEntity);

                // Generate new tokens
                var newAccessToken = _tokenService.GenerateAccessToken(tokenEntity.User);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                var newRefreshTokenEntity = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = tokenEntity.UserId,
                    Token = newRefreshToken,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays),
                    IsRevoked = false
                };

                await _unitOfWork.RefreshTokens.AddAsync(newRefreshTokenEntity);
                await _unitOfWork.CommitTransactionAsync();

                var response = new AuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AppConstants.AccessTokenExpiryMinutes),
                    RequiresTwoFactor = false,
                    User = new UserInfo
                    {
                        Id = tokenEntity.User.Id,
                        PhoneNumber = tokenEntity.User.PhoneNumber,
                        IsTwoFactorEnabled = tokenEntity.User.IsTwoFactorEnabled
                    }
                };

                return BaseResponse<AuthResponse>.SuccessResponse(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error during token refresh");
                return BaseResponse<AuthResponse>.ErrorResponse(
                    "An error occurred during token refresh",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<BaseResponse<TwoFactorStatusResponse>> ToggleTwoFactorAsync(Guid userId, bool enable)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return BaseResponse<TwoFactorStatusResponse>.ErrorResponse("User not found");
                }

                user.IsTwoFactorEnabled = enable;
                user.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                var response = new TwoFactorStatusResponse
                {
                    IsEnabled = enable,
                    Message = enable
                        ? "Two-factor authentication has been enabled"
                        : "Two-factor authentication has been disabled"
                };

                return BaseResponse<TwoFactorStatusResponse>.SuccessResponse(
                    response,
                    response.Message
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling two-factor authentication");
                return BaseResponse<TwoFactorStatusResponse>.ErrorResponse(
                    "An error occurred while updating two-factor authentication",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<BaseResponse<bool>> LogoutAsync(Guid userId)
        {
            try
            {
                await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(userId);
                await _unitOfWork.SaveChangesAsync();

                return BaseResponse<bool>.SuccessResponse(true, "Logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return BaseResponse<bool>.ErrorResponse(
                    "An error occurred during logout",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}
