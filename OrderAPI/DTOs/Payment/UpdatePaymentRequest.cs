using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Payment
{
    public class UpdatePaymentRequest
    {
        [Required(ErrorMessage ="Status is required")]
        public string Status { get; set; } = string.Empty;
    }
}
