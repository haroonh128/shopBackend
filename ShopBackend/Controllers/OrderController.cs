using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Common.enums;
using Shop.Core.DTOs;
using Shop.Core.Interfaces.Services;

namespace ShopBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>Get all orders for the logged-in user</summary>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<OrderResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _orderService.GetAllAsync(GetUserId());
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get order by id</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _orderService.GetByIdAsync(GetUserId(), id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Get all orders for a client</summary>
        [HttpGet("client/{clientId:guid}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<OrderResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByClientId(Guid clientId)
        {
            var result = await _orderService.GetByClientIdAsync(GetUserId(), clientId);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Get all orders with a given status</summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<OrderResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByStatus(OrderStatus status)
        {
            var result = await _orderService.GetByStatusAsync(GetUserId(), status);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Create an order</summary>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponse<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var result = await _orderService.CreateAsync(GetUserId(), request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        /// <summary>Update an order</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrderRequest request)
        {
            var result = await _orderService.UpdateAsync(GetUserId(), id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Update only the status of an order</summary>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(typeof(BaseResponse<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _orderService.UpdateStatusAsync(GetUserId(), id, request);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        /// <summary>Soft-delete an order</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(BaseResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _orderService.DeleteAsync(GetUserId(), id);
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
