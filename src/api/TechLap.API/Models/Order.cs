using TechLap.API.Enums;

namespace TechLap.API.Models
{
    public class Order : BaseModel
    {
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public PaymentMethod Payment { get; set; }
        public OrderStatus Status { get; set; }
        public User User { get; set; } = null!;
        public IEnumerable<OrderDetail> OrderDetails = new List<OrderDetail>();
    }
}
