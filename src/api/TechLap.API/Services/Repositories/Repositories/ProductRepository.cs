using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechLap.API.Data;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class ProductRepository : RepositoryBase, IProductRepository
    {
        public ProductRepository(TechLapContext dbContext) : base(dbContext)
        {
        }
        public async Task<bool> IsCategoryValidAsync(int categoryId)
        {
            return await _dbContext.Categories.AnyAsync(c => c.Id == categoryId);
        }
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsByConditionAsync(Expression<Func<Product, bool>> predicate)
        {
            return await _dbContext.Products.Where(predicate).ToListAsync();
        }

        public async Task<Product> AddAsync(Product entity)
        {
            try
            {
                await _dbContext.Products.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return entity;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Error while adding product: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public async Task<bool> UpdateAsync(Product entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Product entity)
        {
            _dbContext.Products.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
