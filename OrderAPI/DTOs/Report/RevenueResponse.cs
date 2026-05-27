namespace OrderAPI.DTOs.Report
{
    public class RevenueResponse
    {
        public decimal TotalRevenue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int OrderCount { get; set; }
    }
}
