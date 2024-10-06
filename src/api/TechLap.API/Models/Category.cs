using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class Category : BaseModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(255)]
        public string Slug { get; set; } = string.Empty;
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
