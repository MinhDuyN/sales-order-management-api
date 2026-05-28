using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.DTOs.Category;
using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.Order;
using OrderAPI.Entities;
using OrderAPI.Services;
using OrderAPI.Services.Interfaces;
using System.Security.Claims;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrder(string? keyword=null, int? userId=null, DateTime? startDate = null, DateTime? endDate = null, int page=1, int pageSize=5)
        {
            var isCustomer = User.IsInRole("Customer");
            if (isCustomer)
            {
                userId = int.Parse( User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            }
            var orders = await _orderService.GetAllOrder(keyword,userId,startDate,endDate,page,pageSize);
            return Ok(ApiResponse<PagedList<OrderResponse>>.Ok($"Get orders successfully", orders));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var orders = await _orderService.GetOrderById(id);
            return Ok(ApiResponse<OrderResponse>.Ok($"Get orders successfully", orders));
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(int userId, CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var createOrder = await _orderService.CreateOrder(userId, request);
            return CreatedAtAction(nameof(GetOrderById), new { id = createOrder.Id }, ApiResponse<OrderResponse>.Ok("Create Order successfully", createOrder));
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            var updateOrder = await _orderService.UpdateOrder(id,request);
            return Ok(ApiResponse<OrderResponse>.Ok($"Update order successfully", updateOrder));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            await _orderService.DeleteOrder(id);
            return NoContent();
        }
    }
}
