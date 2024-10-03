namespace TechLap.API.Responses
{
    public class GetUserOrdersResponse
    {
        public int Id { get; set; }
        public double TotalPrice { get; set; }
        public string Payment { get; set; }
        public string Status { get; set; }
        public object OrderDetails { get; set; }
    }

    public class OrderDetailDTO
    {
        public string ProductName { get; set; }
        public object Price { get; set; }
        public object Quantity { get; set; }
    }
}
