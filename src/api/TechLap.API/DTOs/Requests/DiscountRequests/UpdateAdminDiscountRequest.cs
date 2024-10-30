using TechLap.API.Enums;

namespace TechLap.API.DTOs.Requests.DiscountRequests
{
    public record UpdateAdminDiscountRequest(
        string DiscountCode,
        decimal DiscountPercentage,
        DateTime StartDate,
        DateTime EndDate,
        int UsageLimit,
        DiscountStatus Status
    );
}

