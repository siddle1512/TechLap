using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechLap.API.Data;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(TechLapContext dbContext) : base(dbContext)
        {
        }

        public Task<User> AddAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<User>> GetAllAsync(Expression<Func<User, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<User> UserLogin(string email, string password)
        {
            string hashedPassword = HashPassword(password);
            var user = await _dbContext.Users.Where(o => o.Email.Equals(email) && o.HashedPassword.Equals(hashedPassword)).FirstAsync();

            if (user == null)
            {
                throw new BadRequestException("Wrong email or password");
            }
            if (user.Status != Enums.UserStatus.Active)
            {
                throw new BadRequestException("User account is not active");
            }
            return user;
        }
    }
}
