using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Auth.RefreshToken
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage ="RefreshToken is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
