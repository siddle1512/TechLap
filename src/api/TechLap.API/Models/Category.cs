namespace TechLap.API.Models
{
    public class Category : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
