using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Slug { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
