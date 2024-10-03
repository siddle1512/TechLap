using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechLap.API.Data;
using TechLap.API.Requests;
using TechLap.API.Responses;

namespace TechLap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(GetUserOrdersRequest request)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == request.UserId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Select(o => new GetUserOrdersResponse
                {
                    Id = o.Id,
                    TotalPrice = o.TotalPrice,
                    Payment = o.Payment.ToString(),
                    Status = o.Status.ToString(),
                    OrderDetails = o.OrderDetails.Select(od => new OrderDetailDTO
                    {
                        ProductName = od.Product.Brand + " " + od.Product.Model,
                        Price = od.Product.Price,
                        Quantity = od.Quantity
                    }).ToList()
                })
                .ToListAsync();

            if (!orders.Any())
            {
                return NotFound($"No orders found for user with ID {request.UserId}");
            }

            return Ok(orders);
        }
    }
}
