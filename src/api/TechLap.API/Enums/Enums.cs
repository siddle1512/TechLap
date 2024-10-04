namespace TechLap.API.Enums
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
        Staff
    }
}
