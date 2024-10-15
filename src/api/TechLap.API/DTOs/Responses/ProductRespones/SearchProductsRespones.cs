namespace TechLap.API.DTOs.Responses.ProductRespones;

public class SearchProductsRespones
{
    // tạo response để trả về sản phẩm tìm kiếm theo cấu hình laptop 
    public int Id { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Cpu { get; set; } = string.Empty;
    public string? Ram { get; set; } = string.Empty;
    public string Vga { get; set; } = string.Empty;
    public string ScreenSize { get; set; } = string.Empty;
    public string HardDisk { get; set; } = string.Empty;
    public string Os { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Image { get; set; } = string.Empty;
}