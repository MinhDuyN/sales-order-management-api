namespace OrderAPI.DTOs.Product
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string CategoryName { get; set; } = string.Empty.ToString();
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
