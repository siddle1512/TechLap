namespace TechLap.API.Models
{
    public class Product : BaseModel
    {
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string Cpu { get; set; } = string.Empty;
        public string Ram { get; set; } = string.Empty;
        public string Vga { get; set; } = string.Empty;
        public string ScreenSize { get; set; } = string.Empty;
        public string HardDisk { get; set; } = string.Empty;
        public string Os { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Amount { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public Category Category { get; set; } = null!;
        public IEnumerable<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
