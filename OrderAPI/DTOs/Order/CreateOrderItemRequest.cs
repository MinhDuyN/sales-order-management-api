using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Order
{
    public class CreateOrderItemRequest
    {
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Quantity is required"),Range(1,100,ErrorMessage ="Quantity must be greater than 0 and less than 100")]
        public int Quantity { get; set; }
    }
}
