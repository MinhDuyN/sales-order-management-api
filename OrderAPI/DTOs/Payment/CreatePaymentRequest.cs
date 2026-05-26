using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Payment
{
    public class CreatePaymentRequest
    {
        [Required(ErrorMessage ="OrderId is required")]
        public int OrderId { get; set; }
        [Required(ErrorMessage = "Method is required")]
        public string Method { get; set; } = string.Empty;
    }
}
