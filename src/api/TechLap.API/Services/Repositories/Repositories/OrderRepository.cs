using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechLap.API.Data;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class OrderRepository : RepositoryBase, IOrderRepository
    {
        public OrderRepository(TechLapContext dbContext) : base(dbContext)
        {
        }

        public async Task<Order> AddAsync(Order entity)
        {
            if (entity.UserId != 0)
            {
                var user = await _dbContext.Users.FindAsync(entity.UserId);
                if (user == null)
                {
                    throw new NotFoundException("User not found");
                }
            }

            if (entity.DiscountId == 0)
            {
                entity.DiscountId = null;
            }
            else if (entity.DiscountId != null)
            {
                var discount = await _dbContext.Discounts.FindAsync(entity.DiscountId);
                if (discount == null)
                {
                    throw new NotFoundException("Discount not found");
                }
            }

            foreach (var detail in entity.OrderDetails)
            {
                var product = await _dbContext.Products.FindAsync(detail.ProductId);
                if (product == null)
                {
                    throw new NotFoundException("Product not found");
                }
            }

            await _dbContext.Orders.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IReadOnlyList<Order>> GetAllAsync(Expression<Func<Order, bool>> predicate)
        {
            var orders = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Discount)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(predicate)
                .ToListAsync();

            if (!orders.Any())
            {
                throw new NotFoundException("No orders found matching the criteria.");
            }
            return orders;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Discount)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new NotFoundException($"Order with ID {id} not found.");
            }

            await CalculateOrderTotal(order);

            return order;
        }

        public async Task<bool> UpdateAsync(Order entity)
        {
            var existingOrder = await _dbContext.Orders.FindAsync(entity.Id);

            if (existingOrder == null) throw new NotFoundException($"Order with ID {entity.Id} not found.");

            existingOrder.OrderDate = entity.OrderDate;
            existingOrder.TotalPrice = entity.TotalPrice;
            existingOrder.Payment = entity.Payment;
            existingOrder.Status = entity.Status;
            existingOrder.DiscountId = entity.DiscountId;

            if (entity.UserId != 0)
            {
                var user = await _dbContext.Users.FindAsync(entity.UserId);
                if (user == null)
                {
                    throw new NotFoundException("User not found");
                }
            }

            if (entity.DiscountId == 0)
            {
                entity.DiscountId = null;
            }
            else if (entity.DiscountId != null)
            {
                var discount = await _dbContext.Discounts.FindAsync(entity.DiscountId);
                if (discount == null)
                {
                    throw new NotFoundException("Discount not found");
                }
            }

            foreach (var detail in entity.OrderDetails)
            {
                var product = await _dbContext.Products.FindAsync(detail.ProductId);
                if (product == null)
                {
                    throw new NotFoundException("Product not found");
                }
            }

            _dbContext.Orders.Update(existingOrder);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public Task<bool> DeleteAsync(Order entity)
        {
            throw new NotImplementedException();
        }

        private async Task CalculateOrderTotal(Order entity)
        {
            entity.TotalPrice = 0;

            if (entity.OrderDetails == null)
            {
                entity.OrderDetails = await _dbContext.OrderDetails
                    .Where(od => od.OrderId == entity.Id)
                    .ToListAsync();
            }

            foreach (var orderDetail in entity.OrderDetails)
            {
                await _dbContext.Entry(orderDetail).Reference(od => od.Product).LoadAsync();

                if (orderDetail.Product != null)
                {
                    orderDetail.Price = orderDetail.Quantity * orderDetail.Product.Price;
                    entity.TotalPrice += orderDetail.Price;
                }
                return;
            }
        }
    }
}
