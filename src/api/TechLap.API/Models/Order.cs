using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TechLap.API.Enums;

namespace TechLap.API.Models
{
    public class Order : BaseModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        [Column(TypeName = "decimal(12,0)")]
        public decimal TotalPrice { get; set; }
        [Required]
        public PaymentMethod Payment { get; set; }
        [Required]
        public OrderStatus Status { get; set; }
        public int? DiscountId { get; set; }
        public Discount Discount { get; set; } = null!;
        public User User { get; set; } = null!;
        public IEnumerable<OrderDetail> OrderDetails = new List<OrderDetail>();
    }
}
