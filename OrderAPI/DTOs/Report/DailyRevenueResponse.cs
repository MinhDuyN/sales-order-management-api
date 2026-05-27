namespace OrderAPI.DTOs.Report
{
    public class DailyRevenueResponse
    {
        public decimal Revenue { get; set; }
        public DateOnly CreatedDate { get; set; }
    }
}
