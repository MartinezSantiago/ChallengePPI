using API.Utils;
using BL.DTOs;
using BL.Services.Order.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderController(IOrderService orderService, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// Retrieves a specific order by its ID.
        /// </summary>
        /// <param name="id">The ID of the order to retrieve.</param>
        /// <returns>The order details.</returns>
        /// <response code="200">Returns the order details if found.</response>
        /// <response code="404">If the order with the provided ID does not exist.</response>
        /// <response code="500">If an unexpected error occurs during the process.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<OrderDTO>>> GetOrderById(int id)
        {
            try
            {
                var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var order = await _orderService.GetOrderByIdAsync(id, userId);
                if (order == null)
                {
                    return NotFound(new ApiResponse<OrderDTO>(false, "Order not found", null));
                }
                return Ok(new ApiResponse<OrderDTO>(true, "Success", order));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<OrderDTO>(false, ex.Message, null));
            }
        }
        /// <summary>
        /// Retrieves all orders associated with the authenticated user.
        /// </summary>
        /// <returns>A list of orders.</returns>
        /// <response code="200">Returns a list of orders.</response>
        /// <response code="500">If an unexpected error occurs during the process.</response>

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<OrderDTO>>>> GetAllOrders()
        {
            try
            {
                var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(new ApiResponse<List<OrderDTO>>(true, "Success", orders));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<OrderDTO>>(false, ex.Message, null));
            }
        }
        /// <summary>
        /// Creates a new order for the authenticated user.
        /// </summary>
        /// <param name="orderDto">The details of the order to create.</param>
        /// <returns>The created order details.</returns>
        /// <response code="200">Returns the created order details.</response>
        /// <response code="400">If the request body is invalid or missing required parameters.</response>
        /// <response code="500">If an unexpected error occurs during the process.</response>

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CreateOrderDTO>>> CreateOrder([FromBody] CreateOrderDTO orderDto)
        {
            try
            {
                var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                orderDto.AccountId = userId;

                await _orderService.CreateOrderAsync(orderDto);
                return Ok(new ApiResponse<CreateOrderDTO>(true, "Order created successfully", orderDto));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<CreateOrderDTO>(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CreateOrderDTO>(false, ex.Message, null));
            }
        }
        /// <summary>
        /// Updates the status of a specific order.
        /// </summary>
        /// <param name="id">The ID of the order to update.</param>
        /// <param name="orderStatusUpdateDto">The updated status of the order.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="400">If the provided IDs do not match.</response>
        /// <response code="500">If an unexpected error occurs during the process.</response>

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderStatusUpdateDTO orderStatusUpdateDto)
        {
            try
            {
                var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                if (id != orderStatusUpdateDto.OrderId)
                {
                    return BadRequest(new ApiResponse<object>(false, "The provided IDs do not match.", null));
                }
                await _orderService.UpdateOrderStatusAsync(orderStatusUpdateDto, userId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, ex.Message, null));
            }
        }

        /// <summary>
        /// Deletes a specific order.
        /// </summary>
        /// <param name="id">The ID of the order to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="400">If an error occurs during deletion.</response>
        /// <response code="500">If an unexpected error occurs during the process.</response>

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                await _orderService.DeleteOrderAsync(id, userId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>(false, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(false, ex.Message, null));
            }
        }
    }
}
