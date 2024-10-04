using System.Linq.Expressions;
using TechLap.API.Models;

namespace TechLap.API.Services.Repositories.IRepositories
{
    public interface IAsyncRepository<T>
    {
        public Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        public Task<T?> GetByIdAsync(int id);
        public Task<T> AddAsync(T entity);
        public Task<bool> UpdateAsync(T entity);
        public Task<bool> DeleteAsync(T entity);
    }
}
