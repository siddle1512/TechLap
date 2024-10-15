using Microsoft.EntityFrameworkCore;
using TechLap.API.DTOs.Requests.DiscountRequests;
using TechLap.API.Models;

namespace TechLap.API.Services.Repositories.IRepositories.Discounts
{
    public interface IDiscountRepository : IAsyncRepository<Discount>
    {
            Task<bool> UpdateDiscountAsync(int id, UpdateAdminDiscountRequest request);
            Task<Discount> ApplyDiscountAsync(string discountCode);






    }
}
