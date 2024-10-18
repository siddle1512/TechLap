namespace TechLap.API.DTOs.Requests
{
    public record ProductRequest(
        string Brand,
        string Model,
        int CategoryId,
        string Cpu,
        string Ram,
        string Vga,
        string ScreenSize,
        string HardDisk,
        string Os,
        decimal Price,
        int Stock,
        string Image
    );
}
