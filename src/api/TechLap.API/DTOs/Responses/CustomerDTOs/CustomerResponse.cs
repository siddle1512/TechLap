namespace TechLap.API.DTOs.Responses.CustomerDTOs
{
    public record CustomerResponse(
        int Id,
        string Name,
        string Email,
        string PhoneNumber
    );
}
