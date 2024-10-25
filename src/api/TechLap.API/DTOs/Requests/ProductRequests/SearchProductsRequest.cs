namespace TechLap.API.DTOs.Requests

{
    public record SearchProductsRequest(
            string Brand,
            string Model,
            string Cpu,
            string Ram,
            string Vga,
            string ScreenSize,
            string HardDisk,
            string Os
       ) ;

}