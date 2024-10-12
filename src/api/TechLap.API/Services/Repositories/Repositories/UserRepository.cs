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

        public async Task<User> AddAsync(User entity)
        {
            var userList = await _dbContext.Users.Where(o => o.Email.ToLower().Contains(entity.Email.ToLower())).ToListAsync();

            if (userList.Any())
            {
                throw new BadRequestException("Email has existed");
            }

            entity.HashedPassword = HashPassword(entity.HashedPassword);
            await _dbContext.Users.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public Task<bool> DeleteAsync(User entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<User>> GetAllAsync(Expression<Func<User, bool>> predicate)
        {
            var users = await _dbContext.Users.Where(predicate).ToListAsync();
            if (!users.Any())
            {
                throw new NotFoundException("Not found any users");
            }
            return await _dbContext.Users.Where(predicate).ToListAsync();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(User entity)
        {
            _dbContext.Users.Entry(entity).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<User> UserLogin(string email, string password)
        {
            string hashedPassword = HashPassword(password);

            var userList = await _dbContext.Users.Where(o => o.Email.Equals(email) && o.HashedPassword.Equals(hashedPassword)).ToListAsync();

            if (!userList.Any())
            {
                throw new BadRequestException("Wrong email or password");
            }
            if (userList.First().Status != Enums.UserStatus.Active)
            {
                throw new BadRequestException("User account is not active");
            }
            return userList.First();
        }
    }
}
