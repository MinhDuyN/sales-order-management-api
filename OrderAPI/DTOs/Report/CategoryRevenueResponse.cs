namespace OrderAPI.DTOs.Report
{
    public class CategoryRevenueResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public int ProductCount { get; set; }
    }
}
