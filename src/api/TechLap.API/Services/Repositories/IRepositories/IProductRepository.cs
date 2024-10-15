using System.Linq.Expressions;
using TechLap.API.Models;

namespace TechLap.API.Services.Repositories.IRepositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IReadOnlyList<Product>> GetProductsByConditionAsync(Expression<Func<Product, bool>> predicate);
        Task<Product> AddAsync(Product entity);
        Task<bool> UpdateAsync(Product entity);
        Task<bool> DeleteAsync(Product entity);
        Task<bool> IsCategoryValidAsync(int categoryId);
    }
}
