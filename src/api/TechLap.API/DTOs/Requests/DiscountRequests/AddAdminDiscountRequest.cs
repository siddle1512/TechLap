namespace TechLap.API.DTOs.Requests.DiscountRequests
{
    public record AddAdminDiscountRequest(
        string DiscountCode,
        decimal DiscountPercentage,
        DateTime EndDate,
        int UsageLimit,
        DateTime LastModifiedDate
    );
}
