using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class Product : BaseModel
    {
        [Required]
        [MaxLength(100)]
        public string Brand { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = string.Empty;
        [Required]
        public int CategoryId { get; set; }
        [MaxLength(100)]
        public string Cpu { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Ram { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Vga { get; set; } = string.Empty;
        [MaxLength(50)]
        public string ScreenSize { get; set; } = string.Empty;
        [MaxLength(100)]
        public string HardDisk { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Os { get; set; } = string.Empty;
        [Required]
        [Column(TypeName = "decimal(12,0)")]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string Image { get; set; } = string.Empty;
        public Category Category { get; set; } = null!;
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
