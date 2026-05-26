namespace OrderAPI.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
