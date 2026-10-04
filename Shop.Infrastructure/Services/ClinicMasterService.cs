using Microsoft.Extensions.Logging;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Repositories;
using Shop.Core.Interfaces.Services;
using Shop.Entities;

namespace Shop.Infrastructure.Services
{
    public class ClinicMasterService : IClinicMasterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPracticeService _practiceService;
        private readonly ILogger<ClinicMasterService> _logger;

        public ClinicMasterService(IUnitOfWork unitOfWork, IPracticeService practiceService, ILogger<ClinicMasterService> logger)
        {
            _unitOfWork = unitOfWork;
            _practiceService = practiceService;
            _logger = logger;
        }

        public async Task<BaseResponse<IEnumerable<DiagnosisResponse>>> SearchDiagnosesAsync(Guid userId, string? query)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var list = (await _unitOfWork.Diagnoses.SearchAsync(practice.Id, query))
                    .Select(d => new DiagnosisResponse { Id = d.Id, Name = d.Name, Code = d.Code, Active = d.Active });
                return BaseResponse<IEnumerable<DiagnosisResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching diagnoses");
                return BaseResponse<IEnumerable<DiagnosisResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<DiagnosisResponse>> CreateDiagnosisAsync(Guid userId, DiagnosisRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BaseResponse<DiagnosisResponse>.ErrorResponse("Diagnosis name is required");

                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var entity = new Diagnosis
                {
                    PracticeId = practice.Id,
                    Name = request.Name.Trim(),
                    Code = request.Code?.Trim(),
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                var added = await _unitOfWork.Diagnoses.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<DiagnosisResponse>.SuccessResponse(
                    new DiagnosisResponse { Id = added.Id, Name = added.Name, Code = added.Code, Active = added.Active },
                    "Diagnosis created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating diagnosis");
                return BaseResponse<DiagnosisResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<TestResponse>>> SearchTestsAsync(Guid userId, string? query)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var list = (await _unitOfWork.Tests.SearchAsync(practice.Id, query))
                    .Select(t => new TestResponse { Id = t.Id, Name = t.Name, Category = t.Category, Active = t.Active });
                return BaseResponse<IEnumerable<TestResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching tests");
                return BaseResponse<IEnumerable<TestResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<TestResponse>> CreateTestAsync(Guid userId, TestRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BaseResponse<TestResponse>.ErrorResponse("Test name is required");

                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var entity = new Test
                {
                    PracticeId = practice.Id,
                    Name = request.Name.Trim(),
                    Category = request.Category?.Trim(),
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                var added = await _unitOfWork.Tests.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<TestResponse>.SuccessResponse(
                    new TestResponse { Id = added.Id, Name = added.Name, Category = added.Category, Active = added.Active },
                    "Test created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating test");
                return BaseResponse<TestResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<IEnumerable<MedicationResponse>>> SearchMedicationsAsync(Guid userId, string? query)
        {
            try
            {
                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var list = (await _unitOfWork.Medications.SearchAsync(practice.Id, query))
                    .Select(m => new MedicationResponse
                    {
                        Id = m.Id,
                        Name = m.Name,
                        GenericName = m.GenericName,
                        Strength = m.Strength,
                        Form = m.Form,
                        Active = m.Active
                    });
                return BaseResponse<IEnumerable<MedicationResponse>>.SuccessResponse(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching medications");
                return BaseResponse<IEnumerable<MedicationResponse>>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }

        public async Task<BaseResponse<MedicationResponse>> CreateMedicationAsync(Guid userId, MedicationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                    return BaseResponse<MedicationResponse>.ErrorResponse("Medication name is required");

                var practice = await _practiceService.GetOrCreatePracticeEntityAsync(userId);
                var entity = new Medication
                {
                    PracticeId = practice.Id,
                    Name = request.Name.Trim(),
                    GenericName = request.GenericName?.Trim(),
                    Strength = request.Strength?.Trim(),
                    Form = request.Form?.Trim(),
                    Active = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow
                };
                var added = await _unitOfWork.Medications.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return BaseResponse<MedicationResponse>.SuccessResponse(
                    new MedicationResponse
                    {
                        Id = added.Id,
                        Name = added.Name,
                        GenericName = added.GenericName,
                        Strength = added.Strength,
                        Form = added.Form,
                        Active = added.Active
                    },
                    "Medication created");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medication");
                return BaseResponse<MedicationResponse>.ErrorResponse("An error occurred", new List<string> { ex.Message });
            }
        }
    }
}
