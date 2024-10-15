using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.OrderDTOs;
using TechLap.API.Exceptions;
using TechLap.API.Mapper;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Controllers
{
    [Route("api/orders")]
    public class OrderController : BaseController<OrderController>
    {
        private readonly IOrderRepository _orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var userId = GetUserIdFromToken();

            var orders = await _orderRepository.GetAllAsync(o => User.IsInRole("Admin") || (userId != null && o.UserId == userId.Value));

            var response = LazyMapper.Mapper.Map<IEnumerable<OrderResponse>>(orders);
            return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userId = GetUserIdFromToken();

            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null || (!User.IsInRole("Admin") && order.UserId != userId.GetValueOrDefault()))
            {
                throw new NotFoundException("Order not found or unauthorized to access this order.");
            }

            var response = LazyMapper.Mapper.Map<OrderResponse>(order);
            return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            var userId = GetUserIdFromToken();

            var order = LazyMapper.Mapper.Map<Order>(request);
            order.UserId = userId.GetValueOrDefault();

            var newOrder = await _orderRepository.AddAsync(order);

            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK, "Add orderId " + order.Id + " successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("adminCreateOrder")]
        public async Task<IActionResult> CreateOrderAdmin([FromBody] OrderAdminRequest request)
        {
            var order = LazyMapper.Mapper.Map<Order>(request);

            var newOrder = await _orderRepository.AddAsync(order);

            return CreateResponse<string>(true, "Order created by admin successfully.", HttpStatusCode.OK, "OrderId " + newOrder.Id + " created for user " + request.UserId);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) throw new NotFoundException("Order not found.");

            var userId = GetUserIdFromToken();
            if (!User.IsInRole("Admin") && order.UserId != userId.GetValueOrDefault())
            {
                throw new AuthenticationException("Unauthorized to update this order.");
            }

            var updatedOrder = LazyMapper.Mapper.Map(request, order);
            await _orderRepository.UpdateAsync(updatedOrder);

            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK, "Update orderId " + order.Id + " successfully");
        }
    }
}
