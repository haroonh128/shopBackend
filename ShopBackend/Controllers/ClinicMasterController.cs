using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicMasterController : ControllerBase
    {
        private readonly IClinicMasterService _clinicMasterService;

        public ClinicMasterController(IClinicMasterService clinicMasterService)
        {
            _clinicMasterService = clinicMasterService;
        }

        [HttpGet("diagnoses")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<DiagnosisResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchDiagnoses([FromQuery] string? q)
        {
            var result = await _clinicMasterService.SearchDiagnosesAsync(GetUserId(), q);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("diagnoses")]
        [ProducesResponseType(typeof(BaseResponse<DiagnosisResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateDiagnosis([FromBody] DiagnosisRequest request)
        {
            var result = await _clinicMasterService.CreateDiagnosisAsync(GetUserId(), request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("tests")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<TestResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchTests([FromQuery] string? q)
        {
            var result = await _clinicMasterService.SearchTestsAsync(GetUserId(), q);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("tests")]
        [ProducesResponseType(typeof(BaseResponse<TestResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTest([FromBody] TestRequest request)
        {
            var result = await _clinicMasterService.CreateTestAsync(GetUserId(), request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("medications")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<MedicationResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchMedications([FromQuery] string? q)
        {
            var result = await _clinicMasterService.SearchMedicationsAsync(GetUserId(), q);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("medications")]
        [ProducesResponseType(typeof(BaseResponse<MedicationResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateMedication([FromBody] MedicationRequest request)
        {
            var result = await _clinicMasterService.CreateMedicationAsync(GetUserId(), request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}
