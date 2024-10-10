using TechLap.API.Models;

namespace TechLap.API.Services.Repositories.IRepositories
{
    public interface IUserRepository : IAsyncRepository<User>
    {
        public Task<User> UserLogin(string email, string password);
    }
}
