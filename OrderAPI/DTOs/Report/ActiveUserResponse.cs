namespace OrderAPI.DTOs.Report
{
    public class ActiveUserResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public DateTime LastOrderDate { get; set; }
    }
}
