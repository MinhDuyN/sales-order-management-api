namespace OrderAPI.DTOs.Report
{
    public class TopUserResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int OrderCount { get; set; }
    }
}
