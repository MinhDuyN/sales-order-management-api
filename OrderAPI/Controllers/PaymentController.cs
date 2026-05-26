using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.Order;
using OrderAPI.DTOs.Payment;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin,Staff")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentById(id);
            return Ok(ApiResponse<PaymentResponse>.Ok($"Get payment successfully", payment));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            var createPayment = await _paymentService.CreatePayment(request);
            return CreatedAtAction(nameof(GetPaymentById), new { id = createPayment.Id }, ApiResponse<PaymentResponse>.Ok("Create payment successfully",createPayment));
 
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            var updatePayment = await _paymentService.UpdatePayment(id, request);
            return Ok(ApiResponse<PaymentResponse>.Ok("Update payment successfully",updatePayment));
            
        }

    }
}
