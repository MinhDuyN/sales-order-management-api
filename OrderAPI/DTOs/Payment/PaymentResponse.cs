using OrderAPI.Entities;

namespace OrderAPI.DTOs.Payment
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
