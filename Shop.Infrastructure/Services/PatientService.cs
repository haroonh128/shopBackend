using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPracticeService _practiceService;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IUnitOfWork unitOfWork, IPracticeService practiceService, ILogger<PatientService> logger)
        {
            _unitOfWork = unitOfWork;
            _practiceService = practiceService;
            _logger = logger;
        }

        public async Task<BaseResponse<PagedResult<PatientResponse>>> SearchAsync(Guid userId, PatientSearchRequest request)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var page = request.Page <= 0 ? 1 : request.Page;
                var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

                var (items, total) = await _unitOfWork.Patients.SearchAsync(
                    practice.Id,
                    request.MRNumber,
                    request.Name,
                    request.ContactNumber,
                    request.Gender,
                    request.DateOfBirth,
                    request.RegistrationFrom,
                    request.RegistrationTo,
                    request.SortBy,
                    request.SortDesc,
                    page,
                    pageSize);

                var mapped = new List<PatientResponse>();
                foreach (var patient in items)
                {
                    mapped.Add(await MapToResponseAsync(practice.Id, patient));
                }

                return BaseResponse<PagedResult<PatientResponse>>.SuccessResponse(new PagedResult<PatientResponse>
                {
                    Items = mapped,
                    TotalCount = total,
                    Page = page,
                    PageSize = pageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching patients for user {UserId}", userId);
                return BaseResponse<PagedResult<PatientResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<PatientResponse>> GetByIdAsync(Guid userId, Guid id)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var patient = await _unitOfWork.Patients.GetByIdForPracticeAsync(practice.Id, id);
                if (patient == null)
                    return BaseResponse<PatientResponse>.ErrorResponse("Patient not found");
                return BaseResponse<PatientResponse>.SuccessResponse(await MapToResponseAsync(practice.Id, patient));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient {Id}", id);
                return BaseResponse<PatientResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<PatientResponse>> CreateAsync(Guid userId, PatientRequest request)
        {
            try
            {
                var errors = Validate(request);
                if (errors.Count > 0)
                    return BaseResponse<PatientResponse>.ErrorResponse("Validation failed", errors);

                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var mrNumber = await GenerateMRNumberAsync(practice.Id);

                var patient = new Patient
                {
                    PracticeId = practice.Id,
                    MRNumber = mrNumber,
                    FullName = request.FullName.Trim(),
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    MaritalStatus = request.MaritalStatus,
                    ContactNumber = request.ContactNumber?.Trim() ?? string.Empty,
                    AlternateContactNumber = request.AlternateContactNumber?.Trim(),
                    Email = request.Email?.Trim(),
                    Address = request.Address?.Trim(),
                    BloodGroup = request.BloodGroup,
                    EmergencyContactName = request.EmergencyContactName?.Trim(),
                    EmergencyContactNumber = request.EmergencyContactNumber?.Trim(),
                    Allergies = request.Allergies?.Trim(),
                    Height = request.Height,
                    Weight = request.Weight,
                    BMI = request.BMI ?? CalculateBmi(request.Height, request.Weight),
                    Notes = request.Notes?.Trim(),
                    CreatedByUserId = userId,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                var added = await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<PatientResponse>.SuccessResponse(await MapToResponseAsync(practice.Id, added), "Patient registered");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient for user {UserId}", userId);
                return BaseResponse<PatientResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<PatientResponse>> UpdateAsync(Guid userId, Guid id, PatientRequest request)
        {
            try
            {
                var errors = Validate(request);
                if (errors.Count > 0)
                    return BaseResponse<PatientResponse>.ErrorResponse("Validation failed", errors);

                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var patient = await _unitOfWork.Patients.GetByIdForPracticeAsync(practice.Id, id);
                if (patient == null)
                    return BaseResponse<PatientResponse>.ErrorResponse("Patient not found");

                patient.FullName = request.FullName.Trim();
                patient.DateOfBirth = request.DateOfBirth;
                patient.Gender = request.Gender;
                patient.MaritalStatus = request.MaritalStatus;
                patient.ContactNumber = request.ContactNumber?.Trim() ?? string.Empty;
                patient.AlternateContactNumber = request.AlternateContactNumber?.Trim();
                patient.Email = request.Email?.Trim();
                patient.Address = request.Address?.Trim();
                patient.BloodGroup = request.BloodGroup;
                patient.EmergencyContactName = request.EmergencyContactName?.Trim();
                patient.EmergencyContactNumber = request.EmergencyContactNumber?.Trim();
                patient.Allergies = request.Allergies?.Trim();
                patient.Height = request.Height;
                patient.Weight = request.Weight;
                patient.BMI = request.BMI ?? CalculateBmi(request.Height, request.Weight);
                patient.Notes = request.Notes?.Trim();
                patient.Active = request.Active;
                patient.IsDeleted = request.IsDeleted;
                patient.ModifiedByUserId = userId;
                patient.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.Patients.Update(patient);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<PatientResponse>.SuccessResponse(await MapToResponseAsync(practice.Id, patient), "Patient updated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient {Id}", id);
                return BaseResponse<PatientResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid userId, Guid id)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var patient = await _unitOfWork.Patients.GetByIdForPracticeAsync(practice.Id, id);
                if (patient == null)
                    return BaseResponse<bool>.ErrorResponse("Patient not found");

                patient.IsDeleted = true;
                patient.Active = false;
                patient.ModifiedByUserId = userId;
                patient.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Patients.Update(patient);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Patient deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private async Task<string> GenerateMRNumberAsync(Guid practiceId)
        {
            var latest = await _unitOfWork.Patients.GetLatestMRNumberAsync(practiceId);
            var next = 1;
            if (!string.IsNullOrWhiteSpace(latest) && latest.StartsWith("MR-", StringComparison.OrdinalIgnoreCase))
            {
                var numeric = latest[3..];
                if (int.TryParse(numeric, out var n))
                    next = n + 1;
            }

            string candidate;
            do
            {
                candidate = $"MR-{next:D6}";
                next++;
            } while (await _unitOfWork.Patients.GetByMRNumberAsync(practiceId, candidate) != null);

            return candidate;
        }

        private async Task<PatientResponse> MapToResponseAsync(Guid practiceId, Patient p)
        {
            var consultations = (await _unitOfWork.Consultations.GetByPatientIdAsync(practiceId, p.Id)).ToList();
            var last = consultations.FirstOrDefault();
            string? lastDoctorName = null;
            if (last != null)
            {
                var doctor = await _unitOfWork.Doctors.GetByIdAsync(last.DoctorId);
                lastDoctorName = doctor?.FullName;
            }

            return new PatientResponse
            {
                Id = p.Id,
                PracticeId = p.PracticeId,
                MRNumber = p.MRNumber,
                FullName = p.FullName,
                DateOfBirth = p.DateOfBirth,
                Age = CalculateAge(p.DateOfBirth),
                Gender = p.Gender,
                MaritalStatus = p.MaritalStatus,
                ContactNumber = p.ContactNumber,
                AlternateContactNumber = p.AlternateContactNumber,
                Email = p.Email,
                Address = p.Address,
                BloodGroup = p.BloodGroup,
                EmergencyContactName = p.EmergencyContactName,
                EmergencyContactNumber = p.EmergencyContactNumber,
                Allergies = p.Allergies,
                Height = p.Height,
                Weight = p.Weight,
                BMI = p.BMI,
                Notes = p.Notes,
                LastVisitDate = last?.VisitDate,
                LastDoctorName = lastDoctorName,
                Active = p.Active,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }

        private static List<string> Validate(PatientRequest request)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(request.FullName))
                errors.Add("Patient name is required");
            if (request.DateOfBirth.HasValue && request.DateOfBirth.Value.Date > DateTime.UtcNow.Date)
                errors.Add("Date of birth cannot be in the future");
            if (request.Height.HasValue && request.Height <= 0)
                errors.Add("Height must be positive");
            if (request.Weight.HasValue && request.Weight <= 0)
                errors.Add("Weight must be positive");
            if (request.BMI.HasValue && request.BMI <= 0)
                errors.Add("BMI must be positive");
            return errors;
        }

        private static decimal? CalculateBmi(decimal? heightCm, decimal? weightKg)
        {
            if (!heightCm.HasValue || !weightKg.HasValue || heightCm <= 0 || weightKg <= 0)
                return null;
            var meters = heightCm.Value / 100m;
            return Math.Round(weightKg.Value / (meters * meters), 2);
        }

        private static int? CalculateAge(DateTime? dob)
        {
            if (!dob.HasValue) return null;
            var today = DateTime.UtcNow.Date;
            var age = today.Year - dob.Value.Year;
            if (dob.Value.Date > today.AddYears(-age)) age--;
            return age < 0 ? 0 : age;
        }
    }
}
