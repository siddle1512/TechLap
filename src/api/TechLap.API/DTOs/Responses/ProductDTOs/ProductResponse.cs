namespace TechLap.API.DTOs.Responses.ProductDTOs
{
    public class ProductResponse
    {
        public int Id { get; set; }
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
        public int Stock { get; set; }
        public string Image { get; set; } = string.Empty;
    }
}
