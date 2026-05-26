using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Product
{
    public class CreateProductRequest
    {
        [Required(ErrorMessage ="ProductName is required")]
        public string ProductName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Price is required")]
        [Range(1,100,ErrorMessage ="Price is range between 1 and 100")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Stock Quantity is required")]
        [Range(1, 100, ErrorMessage = "Stock Quantity is range between 1 and 100")]
        public int StockQuantity { get; set; }
        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
