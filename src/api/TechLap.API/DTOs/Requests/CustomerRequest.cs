namespace TechLap.API.DTOs.Requests
{
    public record CustomerRequest(
        string Name,
        string Email,
        string PhoneNumber
    );
}
