using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPracticeService _practiceService;
        private readonly ILogger<ConsultationService> _logger;

        public ConsultationService(IUnitOfWork unitOfWork, IPracticeService practiceService, ILogger<ConsultationService> logger)
        {
            _unitOfWork = unitOfWork;
            _practiceService = practiceService;
            _logger = logger;
        }

        public async Task<BaseResponse<IEnumerable<ConsultationListItemResponse>>> GetByPatientAsync(Guid userId, Guid patientId)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var patient = await _unitOfWork.Patients.GetByIdForPracticeAsync(practice.Id, patientId);
                if (patient == null)
                    return BaseResponse<IEnumerable<ConsultationListItemResponse>>.ErrorResponse("Patient not found");

                var consultations = await _unitOfWork.Consultations.GetByPatientIdAsync(practice.Id, patientId);
                var result = new List<ConsultationListItemResponse>();
                foreach (var c in consultations)
                {
                    var doctor = await _unitOfWork.Doctors.GetByIdAsync(c.DoctorId);
                    var diagnoses = await _unitOfWork.ConsultationDiagnoses.GetByConsultationIdAsync(c.Id);
                    var primary = diagnoses.FirstOrDefault(d => d.IsPrimary)?.DiagnosisName
                        ?? diagnoses.FirstOrDefault()?.DiagnosisName
                        ?? c.DiagnosisNotes;

                    result.Add(new ConsultationListItemResponse
                    {
                        Id = c.Id,
                        PatientId = c.PatientId,
                        DoctorId = c.DoctorId,
                        DoctorName = doctor?.FullName ?? string.Empty,
                        VisitDate = c.VisitDate,
                        VisitNumber = c.VisitNumber,
                        ChiefComplaint = c.ChiefComplaint,
                        DiagnosisSummary = primary,
                        ConsultationFee = c.ConsultationFee
                    });
                }

                return BaseResponse<IEnumerable<ConsultationListItemResponse>>.SuccessResponse(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing consultations for patient {PatientId}", patientId);
                return BaseResponse<IEnumerable<ConsultationListItemResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ConsultationDetailsResponse>> GetByIdAsync(Guid userId, Guid id)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var consultation = await _unitOfWork.Consultations.GetByIdForPracticeAsync(practice.Id, id);
                if (consultation == null)
                    return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("Consultation not found");

                return BaseResponse<ConsultationDetailsResponse>.SuccessResponse(await MapDetailsAsync(practice, consultation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting consultation {Id}", id);
                return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ConsultationDetailsResponse>> CreateAsync(Guid userId, ConsultationRequest request)
        {
            try
            {
                var errors = Validate(request);
                if (errors.Count > 0)
                    return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("Validation failed", errors);

                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var patient = await _unitOfWork.Patients.GetByIdForPracticeAsync(practice.Id, request.PatientId);
                if (patient == null)
                    return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("Patient not found");

                var doctor = await _unitOfWork.Doctors.GetByIdForPracticeAsync(practice.Id, request.DoctorId);
                if (doctor == null)
                    return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("Doctor not found for this practice");

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    var consultation = new Consultation
                    {
                        PracticeId = practice.Id,
                        PatientId = request.PatientId,
                        DoctorId = request.DoctorId,
                        VisitDate = request.VisitDate == default ? DateTime.UtcNow : request.VisitDate,
                        VisitNumber = await GenerateVisitNumberAsync(practice.Id),
                        ChiefComplaint = request.ChiefComplaint,
                        History = request.History,
                        Examination = request.Examination,
                        Observations = request.Observations,
                        DiagnosisNotes = request.DiagnosisNotes,
                        Advice = request.Advice,
                        FollowUpRequired = request.FollowUpRequired,
                        FollowUpDate = request.FollowUpDate,
                        FollowUpNotes = request.FollowUpNotes,
                        ConsultationFee = request.ConsultationFee,
                        CreatedByUserId = userId,
                        Active = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Consultations.AddAsync(consultation);
                    await _unitOfWork.SaveChangesAsync();

                    await SaveChildrenAsync(consultation.Id, request, replaceExisting: false);
                    await _unitOfWork.CommitTransactionAsync();

                    var saved = await _unitOfWork.Consultations.GetByIdForPracticeAsync(practice.Id, consultation.Id);
                    return BaseResponse<ConsultationDetailsResponse>.SuccessResponse(
                        await MapDetailsAsync(practice, saved!), "Consultation saved");
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating consultation for user {UserId}", userId);
                return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<ConsultationDetailsResponse>> UpdateAsync(Guid userId, Guid id, ConsultationRequest request)
        {
            try
            {
                var errors = Validate(request);
                if (errors.Count > 0)
                    return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("Validation failed", errors);

                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var consultation = await _unitOfWork.Consultations.GetByIdForPracticeAsync(practice.Id, id);
                if (consultation == null)
                    return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("Consultation not found");

                var patient = await _unitOfWork.Patients.GetByIdForPracticeAsync(practice.Id, request.PatientId);
                if (patient == null)
                    return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("Patient not found");

                var doctor = await _unitOfWork.Doctors.GetByIdForPracticeAsync(practice.Id, request.DoctorId);
                if (doctor == null)
                    return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("Doctor not found for this practice");

                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    consultation.PatientId = request.PatientId;
                    consultation.DoctorId = request.DoctorId;
                    consultation.VisitDate = request.VisitDate == default ? consultation.VisitDate : request.VisitDate;
                    consultation.ChiefComplaint = request.ChiefComplaint;
                    consultation.History = request.History;
                    consultation.Examination = request.Examination;
                    consultation.Observations = request.Observations;
                    consultation.DiagnosisNotes = request.DiagnosisNotes;
                    consultation.Advice = request.Advice;
                    consultation.FollowUpRequired = request.FollowUpRequired;
                    consultation.FollowUpDate = request.FollowUpDate;
                    consultation.FollowUpNotes = request.FollowUpNotes;
                    consultation.ConsultationFee = request.ConsultationFee;
                    consultation.ModifiedByUserId = userId;
                    consultation.UpdatedAt = DateTime.UtcNow;

                    _unitOfWork.Consultations.Update(consultation);
                    await SaveChildrenAsync(consultation.Id, request, replaceExisting: true);
                    await _unitOfWork.CommitTransactionAsync();

                    var saved = await _unitOfWork.Consultations.GetByIdForPracticeAsync(practice.Id, consultation.Id);
                    return BaseResponse<ConsultationDetailsResponse>.SuccessResponse(
                        await MapDetailsAsync(practice, saved!), "Consultation updated");
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating consultation {Id}", id);
                return BaseResponse<ConsultationDetailsResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(Guid userId, Guid id)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var consultation = await _unitOfWork.Consultations.GetByIdForPracticeAsync(practice.Id, id);
                if (consultation == null)
                    return BaseResponse<bool>.ErrorResponse("Consultation not found");

                consultation.IsDeleted = true;
                consultation.Active = false;
                consultation.ModifiedByUserId = userId;
                consultation.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Consultations.Update(consultation);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<bool>.SuccessResponse(true, "Consultation deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting consultation {Id}", id);
                return BaseResponse<bool>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        private async Task SaveChildrenAsync(Guid consultationId, ConsultationRequest request, bool replaceExisting)
        {
            if (replaceExisting)
            {
                var existingVitals = await _unitOfWork.ConsultationVitals.GetByConsultationIdAsync(consultationId);
                if (existingVitals != null)
                    _unitOfWork.ConsultationVitals.Remove(existingVitals);

                foreach (var d in await _unitOfWork.ConsultationDiagnoses.GetByConsultationIdAsync(consultationId))
                    _unitOfWork.ConsultationDiagnoses.Remove(d);

                foreach (var t in await _unitOfWork.ConsultationTests.GetByConsultationIdAsync(consultationId))
                    _unitOfWork.ConsultationTests.Remove(t);

                var existingRx = await _unitOfWork.Prescriptions.GetByConsultationIdAsync(consultationId);
                if (existingRx != null)
                {
                    foreach (var item in await _unitOfWork.PrescriptionItems.GetByPrescriptionIdAsync(existingRx.Id))
                        _unitOfWork.PrescriptionItems.Remove(item);
                    _unitOfWork.Prescriptions.Remove(existingRx);
                }

                await _unitOfWork.SaveChangesAsync();
            }

            if (request.Vitals != null)
            {
                await _unitOfWork.ConsultationVitals.AddAsync(new ConsultationVitals
                {
                    ConsultationId = consultationId,
                    Temperature = request.Vitals.Temperature,
                    SystolicBP = request.Vitals.SystolicBP,
                    DiastolicBP = request.Vitals.DiastolicBP,
                    Pulse = request.Vitals.Pulse,
                    RespiratoryRate = request.Vitals.RespiratoryRate,
                    OxygenSaturation = request.Vitals.OxygenSaturation,
                    Height = request.Vitals.Height,
                    Weight = request.Vitals.Weight,
                    BMI = request.Vitals.BMI ?? CalculateBmi(request.Vitals.Height, request.Vitals.Weight),
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            foreach (var d in request.Diagnoses.Where(x => !string.IsNullOrWhiteSpace(x.DiagnosisName)))
            {
                await _unitOfWork.ConsultationDiagnoses.AddAsync(new ConsultationDiagnosis
                {
                    ConsultationId = consultationId,
                    DiagnosisId = d.DiagnosisId,
                    DiagnosisName = d.DiagnosisName.Trim(),
                    IsPrimary = d.IsPrimary,
                    Notes = d.Notes,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            foreach (var t in request.Tests.Where(x => !string.IsNullOrWhiteSpace(x.TestName)))
            {
                await _unitOfWork.ConsultationTests.AddAsync(new ConsultationTest
                {
                    ConsultationId = consultationId,
                    TestId = t.TestId,
                    TestName = t.TestName.Trim(),
                    Notes = t.Notes,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (request.Prescription != null && request.Prescription.Items.Any(i => !string.IsNullOrWhiteSpace(i.MedicationName)))
            {
                var rx = new Prescription
                {
                    ConsultationId = consultationId,
                    PrescriptionDate = request.Prescription.PrescriptionDate == default
                        ? DateTime.UtcNow
                        : request.Prescription.PrescriptionDate,
                    Notes = request.Prescription.Notes,
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Prescriptions.AddAsync(rx);
                await _unitOfWork.SaveChangesAsync();

                var order = 0;
                foreach (var item in request.Prescription.Items.Where(i => !string.IsNullOrWhiteSpace(i.MedicationName)))
                {
                    await _unitOfWork.PrescriptionItems.AddAsync(new PrescriptionItem
                    {
                        PrescriptionId = rx.Id,
                        MedicationId = item.MedicationId,
                        MedicationName = item.MedicationName.Trim(),
                        Strength = item.Strength,
                        Dosage = item.Dosage,
                        Frequency = item.Frequency,
                        Route = item.Route,
                        Duration = item.Duration,
                        Quantity = item.Quantity,
                        Instructions = item.Instructions,
                        SortOrder = item.SortOrder > 0 ? item.SortOrder : order++,
                        Active = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<ConsultationDetailsResponse> MapDetailsAsync(Practice practice, Consultation c)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(c.DoctorId);
            var patient = await _unitOfWork.Patients.GetByIdAsync(c.PatientId);
            var vitals = await _unitOfWork.ConsultationVitals.GetByConsultationIdAsync(c.Id);
            var diagnoses = await _unitOfWork.ConsultationDiagnoses.GetByConsultationIdAsync(c.Id);
            var tests = await _unitOfWork.ConsultationTests.GetByConsultationIdAsync(c.Id);
            var rx = await _unitOfWork.Prescriptions.GetByConsultationIdAsync(c.Id);

            PrescriptionDto? prescription = null;
            if (rx != null)
            {
                var items = await _unitOfWork.PrescriptionItems.GetByPrescriptionIdAsync(rx.Id);
                prescription = new PrescriptionDto
                {
                    Id = rx.Id,
                    PrescriptionDate = rx.PrescriptionDate,
                    Notes = rx.Notes,
                    Items = items.Select(i => new PrescriptionItemDto
                    {
                        Id = i.Id,
                        MedicationId = i.MedicationId,
                        MedicationName = i.MedicationName,
                        Strength = i.Strength,
                        Dosage = i.Dosage,
                        Frequency = i.Frequency,
                        Route = i.Route,
                        Duration = i.Duration,
                        Quantity = i.Quantity,
                        Instructions = i.Instructions,
                        SortOrder = i.SortOrder
                    }).ToList()
                };
            }

            return new ConsultationDetailsResponse
            {
                Id = c.Id,
                PracticeId = c.PracticeId,
                PatientId = c.PatientId,
                DoctorId = c.DoctorId,
                DoctorName = doctor?.FullName ?? string.Empty,
                PatientName = patient?.FullName ?? string.Empty,
                PatientMRNumber = patient?.MRNumber ?? string.Empty,
                VisitDate = c.VisitDate,
                VisitNumber = c.VisitNumber,
                ChiefComplaint = c.ChiefComplaint,
                History = c.History,
                Examination = c.Examination,
                Observations = c.Observations,
                DiagnosisNotes = c.DiagnosisNotes,
                Advice = c.Advice,
                FollowUpRequired = c.FollowUpRequired,
                FollowUpDate = c.FollowUpDate,
                FollowUpNotes = c.FollowUpNotes,
                ConsultationFee = c.ConsultationFee,
                Vitals = vitals == null ? null : new ConsultationVitalsDto
                {
                    Temperature = vitals.Temperature,
                    SystolicBP = vitals.SystolicBP,
                    DiastolicBP = vitals.DiastolicBP,
                    Pulse = vitals.Pulse,
                    RespiratoryRate = vitals.RespiratoryRate,
                    OxygenSaturation = vitals.OxygenSaturation,
                    Height = vitals.Height,
                    Weight = vitals.Weight,
                    BMI = vitals.BMI
                },
                Diagnoses = diagnoses.Select(d => new ConsultationDiagnosisDto
                {
                    Id = d.Id,
                    DiagnosisId = d.DiagnosisId,
                    DiagnosisName = d.DiagnosisName,
                    IsPrimary = d.IsPrimary,
                    Notes = d.Notes
                }).ToList(),
                Tests = tests.Select(t => new ConsultationTestDto
                {
                    Id = t.Id,
                    TestId = t.TestId,
                    TestName = t.TestName,
                    Notes = t.Notes
                }).ToList(),
                Prescription = prescription,
                Doctor = doctor == null ? null : DoctorService.MapToResponse(doctor),
                Practice = new PracticeResponse
                {
                    Id = practice.Id,
                    UserId = practice.UserId,
                    Name = practice.Name,
                    Address = practice.Address,
                    Phone = practice.Phone,
                    Email = practice.Email,
                    Website = practice.Website,
                    LogoUrl = practice.LogoUrl,
                    Active = practice.Active,
                    CreatedAt = practice.CreatedAt,
                    UpdatedAt = practice.UpdatedAt
                },
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            };
        }

        private async Task<string> GenerateVisitNumberAsync(Guid practiceId)
        {
            var latest = await _unitOfWork.Consultations.GetLatestVisitNumberAsync(practiceId);
            var next = 1;
            if (!string.IsNullOrWhiteSpace(latest) && latest.StartsWith("VIS-", StringComparison.OrdinalIgnoreCase))
            {
                var numeric = latest[4..];
                if (int.TryParse(numeric, out var n))
                    next = n + 1;
            }
            return $"VIS-{next:D6}";
        }

        private static List<string> Validate(ConsultationRequest request)
        {
            var errors = new List<string>();
            if (request.PatientId == Guid.Empty)
                errors.Add("Patient is required");
            if (request.DoctorId == Guid.Empty)
                errors.Add("Doctor is required");
            if (request.ConsultationFee < 0)
                errors.Add("Consultation fee cannot be negative");
            if (request.FollowUpRequired && request.FollowUpDate.HasValue && request.FollowUpDate.Value.Date < DateTime.UtcNow.Date.AddDays(-1))
                errors.Add("Follow-up date is invalid");
            if (request.Vitals != null)
            {
                if (request.Vitals.Height.HasValue && request.Vitals.Height <= 0)
                    errors.Add("Height must be positive");
                if (request.Vitals.Weight.HasValue && request.Vitals.Weight <= 0)
                    errors.Add("Weight must be positive");
                if (request.Vitals.BMI.HasValue && request.Vitals.BMI <= 0)
                    errors.Add("BMI must be positive");
            }
            foreach (var item in request.Prescription?.Items ?? new List<PrescriptionItemDto>())
            {
                if (string.IsNullOrWhiteSpace(item.MedicationName) &&
                    (!string.IsNullOrWhiteSpace(item.Dosage) || !string.IsNullOrWhiteSpace(item.Frequency)))
                {
                    errors.Add("Medication name is required when adding a prescription item");
                    break;
                }
            }
            return errors;
        }

        private static decimal? CalculateBmi(decimal? heightCm, decimal? weightKg)
        {
            if (!heightCm.HasValue || !weightKg.HasValue || heightCm <= 0 || weightKg <= 0)
                return null;
            var meters = heightCm.Value / 100m;
            return Math.Round(weightKg.Value / (meters * meters), 2);
        }
    }
}
