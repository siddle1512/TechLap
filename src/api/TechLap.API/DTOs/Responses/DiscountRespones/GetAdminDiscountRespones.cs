namespace TechLap.API.DTOs.Responses.DiscountRespones
{
    public class GetAdminDiscountRespones
    {
        public int Id { get; set; }
        public string DiscountCode { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UsageLimit { get; set; }
        public int TimesUsed { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
