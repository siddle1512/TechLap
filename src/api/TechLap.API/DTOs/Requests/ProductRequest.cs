namespace TechLap.API.DTOs.Requests
{
    public record ProductRequest(
       string Brand,
        string Model,
        int CategoryId,
        string Cpu,
        int Ram,
        string Vga,
        decimal ScreenSize,
        string HardDisk,
        string Os,
        decimal Price,
        int Stock,
        string Image
    );
}
