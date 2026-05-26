using OrderAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.User
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "RoleId is required")]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; } 
        public DateTime CreatedDate { get; set; }
    }
}
