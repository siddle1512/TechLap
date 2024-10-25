using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechLap.API.Data;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories.Discounts;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class DiscountRepository : RepositoryBase, IDiscountRepository
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
            _dbContext.Discounts.Update(discount);
            await _dbContext.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount?> GetByCodeAsync(string discountCode)
        {
            return await _dbContext.Discounts.FirstOrDefaultAsync(d => d.DiscountCode == discountCode);
        }

        public async Task<bool> DeleteAsync(Discount entity)
        {
            var existingDiscount = await _dbContext.Discounts.FindAsync(entity.Id);
            if (existingDiscount == null)
            {
                throw new NotFoundException("Discount not found");
            }

            _dbContext.Discounts.Remove(existingDiscount);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<IReadOnlyList<Discount>> GetAllAsync(Expression<Func<Discount, bool>> predicate)
        {
            var discounts = await _dbContext.Discounts.Where(predicate).ToListAsync();
            if (!discounts.Any())
            {
                throw new NotFoundException("");
            }

            return await _dbContext.Discounts.Where(predicate).ToListAsync();
        }

        public async Task<Discount?> GetByIdAsync(int id)
        {
            var discount = await _dbContext.Discounts.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (discount == null)
            {
                throw new NotFoundException("Not found any discount with id: " + id);
            }

            return discount;
        }

        public async Task<Discount> AddAsync(Discount entity)
        {
            var discountList = await _dbContext.Discounts
                .Where(o => o.DiscountCode.ToLower().Contains(entity.DiscountCode.ToLower())).ToListAsync();
            if (discountList.Any())
            {
                throw new BadRequestException("Discount Code has existed");
            }

            await _dbContext.Discounts.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(Discount entity)
        {
            var existingDiscount = await _dbContext.Discounts.FindAsync(entity.Id);

            if (existingDiscount == null) throw new NotFoundException($"Discount with ID {entity.Id} not found.");

            
            existingDiscount.DiscountCode = entity.DiscountCode;
            existingDiscount.DiscountPercentage = entity.DiscountPercentage;
            existingDiscount.UsageLimit = entity.UsageLimit;
            existingDiscount.EndDate = entity.EndDate;
            existingDiscount.LastModifiedDate = DateTime.Now;

            _dbContext.Discounts.Update(existingDiscount);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        // public async Task<bool> UpdateDiscountAsync(Discount entity)
        // {
        // // Tìm discount hiện có theo id
        // var existingDiscount = await _dbContext.Discounts.FindAsync(id);
        // if (existingDiscount == null)
        // {
        //     throw new NotFoundException("Discount not found");
        // }
        // existingDiscount.DiscountCode = request.DiscountCode;
        // existingDiscount.DiscountPercentage = request.DiscountPercentage;
        // existingDiscount.EndDate = request.EndDate;
        // existingDiscount.UsageLimit = request.UsageLimit;
        // existingDiscount.LastModifiedDate = DateTime.Now;
        // _dbContext.Discounts.Update(existingDiscount);
        // await _dbContext.SaveChangesAsync();
        //
        // return true;
        // }
    }
}