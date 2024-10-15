using TechLap.API.Enums;

namespace TechLap.API.DTOs.Requests
{
    public record OrderRequest(
        DateTime OrderDate,
        decimal TotalPrice,
        PaymentMethod Payment,
        OrderStatus Status,
        int? DiscountId,
        DateTime CreatedDate,
        DateTime LastModifiedDate,
        List<OrderDetailRequest> OrderDetails
    );

    public record OrderAdminRequest(
        int UserId,
        DateTime OrderDate,
        decimal TotalPrice,
        PaymentMethod Payment,
        OrderStatus Status,
        int? DiscountId,
        DateTime CreatedDate,
        DateTime LastModifiedDate,
        List<OrderDetailRequest> OrderDetails
    );

    public record OrderDetailRequest(
        int ProductId,
        int Quantity,
        decimal Price
    );
}
