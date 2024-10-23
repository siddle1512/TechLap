using TechLap.API.Models;

namespace TechLap.API.Services.Repositories.IRepositories
{
    public interface ICategoryRepository : IAsyncRepository<Category>
    {
        public Task<Category?> GetByIdAsync(int id);
    }
}
