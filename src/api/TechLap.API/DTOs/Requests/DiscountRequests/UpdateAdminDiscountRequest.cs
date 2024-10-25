namespace TechLap.API.DTOs.Requests.DiscountRequests
{
    public record UpdateAdminDiscountRequest(
        string DiscountCode,
        decimal DiscountPercentage,
        DateTime EndDate,
        int UsageLimit
    );
}

