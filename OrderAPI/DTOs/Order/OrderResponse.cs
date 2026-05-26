using OrderAPI.Entities;

namespace OrderAPI.DTOs.Order
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<OrderItemResponse> OrderItems { get; set; }
    }
}
