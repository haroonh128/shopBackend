using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StoreItemController : ControllerBase
    {
        private readonly IStoreItemService _storeItemService;

        public StoreItemController(IStoreItemService storeItemService)
        {
            _storeItemService = storeItemService;
        }

        /// <summary>Get all store items</summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<StoreItemResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _storeItemService.GetAllAsync();
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get store items by shop id (Product id)</summary>
        [HttpGet("by-shop/{shopId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<StoreItemResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByShopId(Guid shopId)
        {
            var result = await _storeItemService.GetByShopIdAsync(shopId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get store item by id</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<StoreItemResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _storeItemService.GetByIdAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Create a store item</summary>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<StoreItemResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateStoreItemRequest request)
        {
            var result = await _storeItemService.CreateAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Update a store item</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<StoreItemResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStoreItemRequest request)
        {
            var result = await _storeItemService.UpdateAsync(id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Soft-delete a store item</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _storeItemService.DeleteAsync(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }
    }
}
