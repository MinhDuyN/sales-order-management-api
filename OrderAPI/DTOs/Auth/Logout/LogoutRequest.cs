using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Auth.Logout
{
    public class LogoutRequest
    {
        [Required(ErrorMessage = "RefreshToken is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
