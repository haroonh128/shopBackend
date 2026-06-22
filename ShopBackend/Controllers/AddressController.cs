using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        /// <summary>Get all addresses</summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<AddressResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _addressService.GetAllAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get addresses by addressee id</summary>
        [HttpGet("by-addressee/{addresseeId}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<AddressResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAddresseeId(string addresseeId)
        {
            var result = await _addressService.GetByAddresseeIdAsync(addresseeId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get address by id</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<AddressResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _addressService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Create an address</summary>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<AddressResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAddressRequest request)
        {
            var result = await _addressService.CreateAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Update an address</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<AddressResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressRequest request)
        {
            var result = await _addressService.UpdateAsync(id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Soft-delete an address</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _addressService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
