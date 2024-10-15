using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechLap.API.Data;
using TechLap.API.DTOs.Requests.DiscountRequests;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories.Discounts;

namespace TechLap.API.Services.Repositories.Repositories.Discounts
{
    public class DiscountRepository :  RepositoryBase, IDiscountRepository
    {
        public DiscountRepository(TechLapContext dbContext) : base(dbContext)
        {
        }
       
        public async Task<Discount> ApplyDiscountAsync(string discountCode)
        {
            var discount = await GetByCodeAsync(discountCode);
            if (discount == null)
            {
                throw new NotFoundException("Discount code is invalid.");
            }
            if (discount.StartDate > DateTime.Now)
            {
                throw new Exception("Discount code is not yet valid.");
            }
            if (discount.EndDate < DateTime.Now)
            {
                throw new Exception("Discount code has expired.");
            }
            if (discount.TimesUsed >= discount.UsageLimit)
            {
                throw new Exception("Discount code has been used up.");
            }
            discount.TimesUsed += 1;
            discount.LastModifiedDate = DateTime.Now;
            _dbContext.Discount.Update(discount);
            await _dbContext.SaveChangesAsync();
            return discount;
        }
        public async Task<Discount?> GetByCodeAsync(string discountCode)
        {
            return await _dbContext.Discount.FirstOrDefaultAsync(d => d.DiscountCode == discountCode);
        }
        
        public async Task<bool> DeleteAsync(Discount entity)
        {
            var existingDiscount = await _dbContext.Discount.FindAsync(entity.Id);
            if (existingDiscount == null)
            {
                throw new NotFoundException("Discount not found");
            }
            _dbContext.Discount.Remove(existingDiscount);
            await _dbContext.SaveChangesAsync();
    
            return true;
        }


        public async Task<IReadOnlyList<Discount>> GetAllAsync(Expression<Func<Discount, bool>> predicate)
        {
            var discounts = await _dbContext.Discount.Where(predicate).ToListAsync();
            if (!discounts.Any())
            {
                throw new NotFoundException("");
            }
            return await _dbContext.Discount.Where(predicate).ToListAsync();
        }

        public async Task<Discount?> GetByIdAsync(int id)
        {
            var discount = await _dbContext.Discount.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (discount == null)
            {
                throw new NotFoundException("Not found any discount with id: " + id);
            }
            return discount;
        }

        public async Task<Discount> AddAsync(Discount entity)
        {
            var discountList = await _dbContext.Discount.Where(o => o.DiscountCode.ToLower().Contains(entity.DiscountCode.ToLower())).ToListAsync();
            if (discountList.Any())
            {
                throw new BadRequestException("Discount Code has existed");
            }
            await _dbContext.Discount.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
        
        public async Task<bool> UpdateAsync(Discount entity)
        {
            // var existingDiscount = await GetByIdAsync(entity.Id); 
            // if (existingDiscount == null)
            // {
            //     throw new NotFoundException("Discount not found");
            // }
            
            _dbContext.Discount.Update(entity);
            await _dbContext.SaveChangesAsync();
            return true; 
        }
        
        public async Task<bool> UpdateDiscountAsync(int id, UpdateAdminDiscountRequest request)
        {
            // Tìm discount hiện có theo id
            var existingDiscount = await _dbContext.Discount.FindAsync(id);
            if (existingDiscount == null)
            {
                throw new NotFoundException("Discount not found");
            }
            existingDiscount.DiscountCode = request.DiscountCode;
            existingDiscount.DiscountPercentage = request.DiscountPercentage;
            existingDiscount.EndDate = request.EndDate;
            existingDiscount.UsageLimit = request.UsageLimit;
            existingDiscount.LastModifiedDate = DateTime.Now;
            _dbContext.Discount.Update(existingDiscount);
            await _dbContext.SaveChangesAsync();
    
            return true;
        }

    }
}
