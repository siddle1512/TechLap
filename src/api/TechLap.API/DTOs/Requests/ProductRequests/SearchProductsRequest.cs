namespace TechLap.API.DTOs.Requests

{
    public record SearchProductsRequest(
            int id,
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