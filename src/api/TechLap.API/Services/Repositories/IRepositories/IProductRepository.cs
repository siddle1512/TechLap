using TechLap.API.DTOs.Requests;
using TechLap.API.Models;

namespace TechLap.API.Services.Repositories.IRepositories
{
    public interface IProductRepository : IAsyncRepository<Product>
    {
        Task<IReadOnlyList<Product>> SearchProductsAsync(SearchProductsRequest request);
        Task<bool> IsCategoryValidAsync(int categoryId);
    }
}
