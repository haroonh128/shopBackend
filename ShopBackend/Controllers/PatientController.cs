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
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IConsultationService _consultationService;

        public PatientController(IPatientService patientService, IConsultationService consultationService)
        {
            _patientService = patientService;
            _consultationService = consultationService;
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(BaseResponse<PagedResult<PatientResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] PatientSearchRequest request)
        {
            var result = await _patientService.SearchAsync(GetUserId(), request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<PatientResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _patientService.GetByIdAsync(GetUserId(), id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpGet("{patientId:guid}/consultations")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ConsultationListItemResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConsultations(Guid patientId)
        {
            var result = await _consultationService.GetByPatientAsync(GetUserId(), patientId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<PatientResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] PatientRequest request)
        {
            var result = await _patientService.CreateAsync(GetUserId(), request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<PatientResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] PatientRequest request)
        {
            var result = await _patientService.UpdateAsync(GetUserId(), id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _patientService.DeleteAsync(GetUserId(), id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userIdClaim!);
        }
    }
}
