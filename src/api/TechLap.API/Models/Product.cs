using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public string Cpu { get; set; }
        public string Ram { get; set; }
        public string Vga { get; set; }
        public string ScreenSize { get; set; }
        public string HardDisk { get; set; }
        public string Os { get; set; }
        public double Price { get; set; }
        public string Amount { get; set; }
        public string Image { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
