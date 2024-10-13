using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechLap.API.Data;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class AdminRepository : RepositoryBase, IAdminRepository
    {
        public AdminRepository(TechLapContext dbContext) : base(dbContext)
        {
        }

        public Task<Admin> AddAsync(Admin entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Admin> AdminLogin(string username, string password)
        {
            string hashedPassword = HashPassword(password);

            var admin = await _dbContext.Admins.Where(o => o.Username.Equals(username) && o.HashedPassword.Equals(hashedPassword)).FirstOrDefaultAsync();

            if (admin == null)
            {
                throw new BadRequestException("Wrong username or password");
            }
            return admin;
        }

        public Task<bool> DeleteAsync(Admin entity)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Admin>> GetAllAsync(Expression<Func<Admin, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Admin?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(Admin entity)
        {
            throw new NotImplementedException();
        }
    }
}
