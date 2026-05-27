using OrderAPI.DTOs.Report;

namespace OrderAPI.Services.Interfaces
{
    public interface IReportService
    {
        Task<RevenueResponse> RevenueReport(DateTime startDate, DateTime endDate);
        Task<List<DailyRevenueResponse>> DailyRevenueReport(DateTime startDate, DateTime endDate);
        Task<List<MonthRevenueResponse>> MonthRevenueReport(DateTime startDate, DateTime endDate);
        Task<List<TopProductResponse>> TopProductReport(DateTime startDate, DateTime endDate, int top);
        Task<List<TopUserResponse>> TopUserReport(DateTime startDate, DateTime endDate, int top);
        Task<List<CategoryRevenueResponse>> CategoryRevenueReport(DateTime startDate, DateTime endDate);
        Task<List<ActiveUserResponse>> ActiveUserReport(DateTime startDate, DateTime endDate);
    }
}
