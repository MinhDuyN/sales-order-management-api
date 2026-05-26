using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Category
{
    public class UpdateCategoryRequest
    {
        [Required(ErrorMessage ="CategoryName is required")]
        public string CategoryName { get; set; } = string.Empty;
    }
}
