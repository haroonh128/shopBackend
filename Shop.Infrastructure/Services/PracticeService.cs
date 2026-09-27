using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class PracticeService : IPracticeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PracticeService> _logger;

        public PracticeService(IUnitOfWork unitOfWork, ILogger<PracticeService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<BaseResponse<PracticeResponse>> GetCurrentAsync(Guid userId)
        {
            try
            {
                var practice = await GetOrCreatePracticeEntityAsync(userId);
                return BaseResponse<PracticeResponse>.SuccessResponse(MapToResponse(practice));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting practice for user {UserId}", userId);
                return BaseResponse<PracticeResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<PracticeResponse>> UpdateCurrentAsync(Guid userId, PracticeRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BaseResponse<PracticeResponse>.ErrorResponse("Practice name is required");

                var practice = await GetOrCreatePracticeEntityAsync(userId);
                var tracked = await _unitOfWork.Practices.GetByIdAsync(practice.Id);
                if (tracked == null)
                    return BaseResponse<PracticeResponse>.ErrorResponse("Practice not found");

                tracked.Name = request.Name.Trim();
                tracked.Address = request.Address?.Trim() ?? string.Empty;
                tracked.Phone = request.Phone?.Trim() ?? string.Empty;
                tracked.Email = request.Email?.Trim() ?? string.Empty;
                tracked.Website = request.Website?.Trim() ?? string.Empty;
                tracked.LogoUrl = request.LogoUrl;
                tracked.Active = request.Active;
                tracked.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Practices.Update(tracked);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<PracticeResponse>.SuccessResponse(MapToResponse(tracked), "Practice updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating practice for user {UserId}", userId);
                return BaseResponse<PracticeResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<Practice> GetOrCreatePracticeEntityAsync(Guid userId)
        {
            var existing = await _unitOfWork.Practices.GetByUserIdAsync(userId);
            if (existing != null)
            {
                var tracked = await _unitOfWork.Practices.GetByIdAsync(existing.Id);
                return tracked ?? existing;
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            var practice = new Practice
            {
                UserId = userId,
                Name = user != null
                    ? $"{user.FirstName} {user.LastName}".Trim().Length > 0
                        ? $"{user.FirstName} {user.LastName}".Trim() + " Clinic"
                        : "My Clinic"
                    : "My Clinic",
                Phone = user?.PhoneNumber ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                Active = true,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Practices.AddAsync(practice);
            await _unitOfWork.SaveChangesAsync();

            var defaultTests = new[]
            {
                "CBC", "Blood Sugar", "Lipid Profile", "LFT", "RFT", "X-Ray Chest", "Ultrasound", "ECG", "CRP"
            };
            foreach (var name in defaultTests)
            {
                await _unitOfWork.Tests.AddAsync(new Test
                {
                    PracticeId = practice.Id,
                    Name = name,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            var defaultMeds = new[]
            {
                ("Paracetamol", "500mg", "Tablet"),
                ("Amoxicillin", "500mg", "Capsule"),
                ("Ibuprofen", "400mg", "Tablet"),
                ("Omeprazole", "20mg", "Capsule"),
                ("Cetirizine", "10mg", "Tablet")
            };
            foreach (var (name, strength, form) in defaultMeds)
            {
                await _unitOfWork.Medications.AddAsync(new Medication
                {
                    PracticeId = practice.Id,
                    Name = name,
                    Strength = strength,
                    Form = form,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return practice;
        }

        private static PracticeResponse MapToResponse(Practice p) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            Name = p.Name,
            Address = p.Address,
            Phone = p.Phone,
            Email = p.Email,
            Website = p.Website,
            LogoUrl = p.LogoUrl,
            Active = p.Active,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}
