namespace OrderAPI.Entities
{
    public class Order
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public User? User { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
