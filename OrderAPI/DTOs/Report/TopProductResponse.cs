namespace OrderAPI.DTOs.Report
{
    public class TopProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
        public DateOnly CreatedDate { get; set; }
    }
}
