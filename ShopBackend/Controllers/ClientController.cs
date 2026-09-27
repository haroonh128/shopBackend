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
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>Get all clients for the logged-in user</summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ClientResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clientService.GetAllAsync(GetUserId());
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get clients by module id for the logged-in user</summary>
        [HttpGet("by-module/{moduleId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<ClientResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByModuleId(Guid moduleId)
        {
            var result = await _clientService.GetByModuleIdAsync(GetUserId(), moduleId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get client by id</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<ClientResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _clientService.GetByIdAsync(GetUserId(), id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Create a client</summary>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<ClientResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] ClientRequest request)
        {
            var result = await _clientService.CreateAsync(GetUserId(), request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Update a client</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<ClientResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ClientRequest request)
        {
            var result = await _clientService.UpdateAsync(GetUserId(), id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Soft-delete a client</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _clientService.DeleteAsync(GetUserId(), id);
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
