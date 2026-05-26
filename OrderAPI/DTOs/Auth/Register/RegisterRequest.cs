using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Auth.Register
{
    public class RegisterRequest
    {
        [Required(ErrorMessage ="Name is required")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage ="Email is not valid")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6,ErrorMessage ="Minlenght's password is 6 characters")]
        public string PasswordHash { get; set; } = string.Empty;
    }
}
