using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechLap.API.Data;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class CategoryRepository : RepositoryBase, ICategoryRepository
    {
        public CategoryRepository(TechLapContext dbContext) : base(dbContext)
        {
        }

        public async Task<Category> AddAsync(Category entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(Category entity)
        {
            _dbContext.Categories.Remove(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<IReadOnlyList<Category>> GetAllAsync(Expression<Func<Category, bool>> predicate)
        {
            var categories = await _dbContext.Categories.Where(predicate).ToListAsync();
            if (!categories.Any())
            {
                throw new NotFoundException("Not found any categories");
            }
            return categories;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            var category = await _dbContext.Categories.Where(o => o.Id == id).FirstOrDefaultAsync();
            if (category == null)
            {
                throw new NotFoundException("Not found any categories with " + id);
            }
            return category;
        }

        public async Task<bool> UpdateAsync(Category entity)
        {
            _dbContext.Categories.Entry(entity).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
