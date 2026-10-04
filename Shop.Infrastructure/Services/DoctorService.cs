using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPracticeService _practiceService;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(IUnitOfWork unitOfWork, IPracticeService practiceService, ILogger<DoctorService> logger)
        {
            _unitOfWork = unitOfWork;
            _practiceService = practiceService;
            _logger = logger;
        }

        public async Task<BaseResponse<IEnumerable<DoctorResponse>>> GetAllAsync(Guid userId)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var list = (await _unitOfWork.Doctors.GetByPracticeIdAsync(practice.Id))
                    .Select(MapToResponse)
                    .ToList();
                return BaseResponse<IEnumerable<DoctorResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing doctors for user {UserId}", userId);
                return BaseResponse<IEnumerable<DoctorResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<DoctorResponse>> GetByIdAsync(Guid userId, Guid id)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var doctor = await _unitOfWork.Doctors.GetByIdForPracticeAsync(practice.Id, id);
                if (doctor == null)
                    return BaseResponse<DoctorResponse>.ErrorResponse("Doctor not found");
                return BaseResponse<DoctorResponse>.SuccessResponse(MapToResponse(doctor));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor {Id}", id);
                return BaseResponse<DoctorResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<DoctorResponse>> CreateAsync(Guid userId, DoctorRequest request)
        {
            try
            {
                var errors = Validate(request);
                if (errors.Count > 0)
                    return BaseResponse<DoctorResponse>.ErrorResponse("Validation failed", errors);

                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var doctor = new Doctor
                {
                    PracticeId = practice.Id,
                    UserId = request.UserId,
                    FullName = request.FullName.Trim(),
                    Qualifications = request.Qualifications?.Trim() ?? string.Empty,
                    Specialization = request.Specialization?.Trim() ?? string.Empty,
                    RegistrationNumber = request.RegistrationNumber?.Trim() ?? string.Empty,
                    Phone = request.Phone?.Trim() ?? string.Empty,
                    Email = request.Email?.Trim() ?? string.Empty,
                    Address = request.Address?.Trim() ?? string.Empty,
                    DefaultConsultationFee = request.DefaultConsultationFee,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                var added = await _unitOfWork.Doctors.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<DoctorResponse>.SuccessResponse(MapToResponse(added), "Doctor created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor for user {UserId}", userId);
                return BaseResponse<DoctorResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<DoctorResponse>> UpdateAsync(Guid userId, Guid id, DoctorRequest request)
        {
            try
            {
                var errors = Validate(request);
                if (errors.Count > 0)
                    return BaseResponse<DoctorResponse>.ErrorResponse("Validation failed", errors);

                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var doctor = await _unitOfWork.Doctors.GetByIdForPracticeAsync(practice.Id, id);
                if (doctor == null)
                    return BaseResponse<DoctorResponse>.ErrorResponse("Doctor not found");

                doctor.UserId = request.UserId;
                doctor.FullName = request.FullName.Trim();
                doctor.Qualifications = request.Qualifications?.Trim() ?? string.Empty;
                doctor.Specialization = request.Specialization?.Trim() ?? string.Empty;
                doctor.RegistrationNumber = request.RegistrationNumber?.Trim() ?? string.Empty;
                doctor.Phone = request.Phone?.Trim() ?? string.Empty;
                doctor.Email = request.Email?.Trim() ?? string.Empty;
                doctor.Address = request.Address?.Trim() ?? string.Empty;
                doctor.DefaultConsultationFee = request.DefaultConsultationFee;
                doctor.Active = request.Active;
                doctor.IsDeleted = request.IsDeleted;
                doctor.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Doctors.Update(doctor);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<DoctorResponse>.SuccessResponse(MapToResponse(doctor), "Doctor updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor {Id}", id);
                return BaseResponse<DoctorResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid userId, Guid id)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var doctor = await _unitOfWork.Doctors.GetByIdForPracticeAsync(practice.Id, id);
                if (doctor == null)
                    return BaseResponse<bool>.ErrorResponse("Doctor not found");

                doctor.IsDeleted = true;
                doctor.Active = false;
                doctor.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Doctors.Update(doctor);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Doctor deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private static List<string> Validate(DoctorRequest request)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(request.FullName))
                errors.Add("Doctor name is required");
            if (request.DefaultConsultationFee < 0)
                errors.Add("Default consultation fee cannot be negative");
            return errors;
        }

        internal static DoctorResponse MapToResponse(Doctor d) => new()
        {
            Id = d.Id,
            PracticeId = d.PracticeId,
            UserId = d.UserId,
            FullName = d.FullName,
            Qualifications = d.Qualifications,
            Specialization = d.Specialization,
            RegistrationNumber = d.RegistrationNumber,
            Phone = d.Phone,
            Email = d.Email,
            Address = d.Address,
            DefaultConsultationFee = d.DefaultConsultationFee,
            Active = d.Active,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt
        };
    }
}
