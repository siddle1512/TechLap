using TechLap.API.Enums;

namespace TechLap.API.DTOs.Responses.OrderDTOs
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public int? DiscountId { get; set; }
        public IEnumerable<OrderDetailResponse> OrderDetails { get; set; } = new List<OrderDetailResponse>();
        public IEnumerable<UserResponse> Users { get; set; } = new List<UserResponse>();
        public IEnumerable<DiscountResponse> Discounts { get; set; } = new List<DiscountResponse>();
        public IEnumerable<ProductResponse> Products { get; set; } = new List<ProductResponse>();
    }

    public class OrderDetailResponse
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class UserResponse
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthYear { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AvatarPath { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
    }

    public class DiscountResponse
    {
        public string DiscountCode { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int TimesUsed { get; set; }
        public DiscountStatus Status { get; set; }
    }

    public class ProductResponse
    {
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Cpu { get; set; } = string.Empty;
        public string Ram { get; set; } = string.Empty;
        public string Vga { get; set; } = string.Empty;
        public string ScreenSize { get; set; } = string.Empty;
        public string HardDisk { get; set; } = string.Empty;
        public string Os { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; } = string.Empty;
    }
}
