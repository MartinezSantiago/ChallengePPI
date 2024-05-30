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
