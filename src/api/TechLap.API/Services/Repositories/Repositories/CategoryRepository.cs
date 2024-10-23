using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public Task<Category> AddAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Category entity)
        {
            throw new NotImplementedException();
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

        public Task<bool> UpdateAsync(Category entity)
        {
            throw new NotImplementedException();
        }
    }
}
