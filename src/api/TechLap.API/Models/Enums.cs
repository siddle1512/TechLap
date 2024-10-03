namespace TechLap.API.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Suspended
    }

    public enum PaymentMethod
    {
        CreditCard,
        PayPal,
        BankTransfer
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public enum AdminRole
    {
        SuperAdmin,
        Manager,
        Support
    }
}
