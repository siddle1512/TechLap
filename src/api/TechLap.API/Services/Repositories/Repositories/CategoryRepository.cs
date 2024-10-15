using Microsoft.EntityFrameworkCore;
using TechLap.API.Data;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly TechLapContext _dbContext;

        public CategoryRepository(TechLapContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _dbContext.Categories.FindAsync(id);
        }
    }
}
