using System.ComponentModel.DataAnnotations;
using TechLap.API.Enums;

namespace TechLap.API.Models
{
    public class Discount : BaseModel
    {
        [Required]
        public string DiscountCode { get; set; } = string.Empty;
        [Required]
        public decimal DiscountPercentage { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int UsageLimit { get; set; }
        [Required]
        public int TimesUsed { get; set; }
        public DiscountStatus Status { get; set; }
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
