using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Order
{
    public class UpdateOrderRequest
    {
        [Required(ErrorMessage ="Status is required")]
        public string Status { get; set; } = string.Empty;
    }
}
