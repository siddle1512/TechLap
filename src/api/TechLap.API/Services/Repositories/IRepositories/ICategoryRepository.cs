using TechLap.API.Models;

namespace TechLap.API.Services.Repositories.IRepositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
    }
}
