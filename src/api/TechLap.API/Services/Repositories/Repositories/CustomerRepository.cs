using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechLap.API.Data;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class CustomerRepository : RepositoryBase, ICustomerRepository
    {
        public CustomerRepository(TechLapContext dbContext) : base(dbContext)
        {
        }

        public async Task<Customer> AddAsync(Customer entity)
        {
            await _dbContext.Customers.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IReadOnlyList<Customer>> GetAllAsync(Expression<Func<Customer, bool>> predicate)
        {
            return await _dbContext.Customers.Where(predicate).ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            var customer = await _dbContext.Customers.FindAsync(id);

            if (customer == null)
            {
                throw new NotFoundException($"Customer with ID {id} not found.");
            }

            return customer;
        }

        public async Task<bool> UpdateAsync(Customer entity)
        {
            var existingCustomer = await _dbContext.Customers.FindAsync(entity.Id);

            if (existingCustomer == null)
            {
                throw new NotFoundException($"Customer with ID {entity.Id} not found.");
            }

            existingCustomer.Name = entity.Name;
            existingCustomer.Email = entity.Email;
            existingCustomer.PhoneNumber = entity.PhoneNumber;

            _dbContext.Customers.Update(existingCustomer);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Customer entity)
        {
            _dbContext.Customers.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
