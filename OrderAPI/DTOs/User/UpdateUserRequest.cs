using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.User
{
    public class UpdateUserRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "RoleId is required")]
        public int RoleId { get; set; }
        [Required(ErrorMessage = "IsActive is required")]
        public bool IsActive { get; set; }
    }
}
