using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.DTOs.Common;
using OrderAPI.DTOs.Report;
using OrderAPI.Services.Interfaces;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> RevenueReport(DateTime startDate, DateTime endDate)
        {
            var data = await _reportService.RevenueReport(startDate, endDate);
            return Ok(ApiResponse<RevenueResponse>.Ok("Revenue report retrieved successfully", data));
        }

        [HttpGet("revenue/daily")]
        public async Task<IActionResult> DailyRevenueReport(DateTime startDate, DateTime endDate)
        {
            var data = await _reportService.DailyRevenueReport(startDate, endDate);
            return Ok(ApiResponse<List<DailyRevenueResponse>>.Ok("Daily revenue report retrieved successfully", data));
        }

        [HttpGet("revenue/monthly")]
        public async Task<IActionResult> MonthRevenueReport(DateTime startDate, DateTime endDate)
        {
            var data = await _reportService.MonthRevenueReport(startDate, endDate);
            return Ok(ApiResponse<List<MonthRevenueResponse>>.Ok("Monthly revenue report retrieved successfully", data));
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> TopProductReport(DateTime startDate, DateTime endDate, int top = 10)
        {
            var data = await _reportService.TopProductReport(startDate, endDate, top);
            return Ok(ApiResponse<List<TopProductResponse>>.Ok("Top products report retrieved successfully", data));
        }

        [HttpGet("top-users")]
        public async Task<IActionResult> TopUserReport(DateTime startDate, DateTime endDate, int top = 10)
        {
            var data = await _reportService.TopUserReport(startDate, endDate, top);
            return Ok(ApiResponse<List<TopUserResponse>>.Ok("Top customers report retrieved successfully", data));
        }

        [HttpGet("category-revenue")]
        public async Task<IActionResult> CategoryRevenueReport(DateTime startDate, DateTime endDate)
        {
            var data = await _reportService.CategoryRevenueReport(startDate, endDate);
            return Ok(ApiResponse<List<CategoryRevenueResponse>>.Ok("Category revenue report retrieved successfully", data));
        }
        
        [HttpGet("active-users")]
        public async Task<IActionResult> ActiveUserReport(DateTime startDate, DateTime endDate)
        {
            var data = await _reportService.ActiveUserReport(startDate, endDate);
            return Ok(ApiResponse<List<ActiveUserResponse>>.Ok("Active users report retrieved successfully", data));
        }
    }
}
