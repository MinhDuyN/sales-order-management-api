using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DTOs.Category
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage ="Category Name is required")]
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
