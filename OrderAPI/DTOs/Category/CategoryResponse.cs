namespace OrderAPI.DTOs.Category
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
