using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechLap.API.Data;
using TechLap.API.DTOs.Requests;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Services.Repositories.Repositories;

public class ProductRepository : RepositoryBase, IProductRepository
{
    public ProductRepository(TechLapContext dbContext) : base(dbContext)
    {
    }

    public async Task<Product> AddAsync(Product entity)
    {
        await _dbContext.Products.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Product entity)
    {
        _dbContext.Products.Remove(entity);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(Expression<Func<Product, bool>> predicate)
    {
        var products = await _dbContext.Products.Where(predicate).ToListAsync();
        if (!products.Any())
        {
            throw new NotFoundException("");
        }
        return await _dbContext.Products.Where(predicate).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbContext.Products.FindAsync(id);
    }

    public async Task<bool> UpdateAsync(Product entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        return await _dbContext.SaveChangesAsync() > 0;
    }
    
    public async Task<IReadOnlyList<Product>> SearchProductsAsync(SearchProductsRequest request)
    {
        var query = _dbContext.Products.AsQueryable();

        // Tạo một danh sách các từ khóa tìm kiếm từ các thuộc tính đã nhập
        var searchTerms = new List<string>();

        // Chỉ thêm các thuộc tính không rỗng vào danh sách
        if (!string.IsNullOrEmpty(request.Brand)) searchTerms.Add(request.Brand.ToLower());
        if (!string.IsNullOrEmpty(request.Model)) searchTerms.Add(request.Model.ToLower());
        if (!string.IsNullOrEmpty(request.Cpu)) searchTerms.Add(request.Cpu.ToLower());
        if (!string.IsNullOrEmpty(request.Ram)) searchTerms.Add(request.Ram.ToLower());
        if (!string.IsNullOrEmpty(request.Vga)) searchTerms.Add(request.Vga.ToLower());
        if (!string.IsNullOrEmpty(request.ScreenSize)) searchTerms.Add(request.ScreenSize.ToLower());
        if (!string.IsNullOrEmpty(request.HardDisk)) searchTerms.Add(request.HardDisk.ToLower());
        if (!string.IsNullOrEmpty(request.Os)) searchTerms.Add(request.Os.ToLower());

        if (!searchTerms.Any()) return await _dbContext.Products.ToListAsync();

        query = query.Where(product =>
            searchTerms.Any(term =>
                product.Brand.ToLower().Contains(term) ||
                product.Model.ToLower().Contains(term) ||
                product.Cpu.ToLower().Contains(term) ||
                product.Ram.ToLower().Contains(term) ||
                product.Vga.ToLower().Contains(term) ||
                product.ScreenSize.ToLower().Contains(term) ||
                product.HardDisk.ToLower().Contains(term) ||
                product.Os.ToLower().Contains(term)));

        var result = await query.ToListAsync();
        if (!result.Any()) throw new NotFoundException("No matching products found.");

        return result;
    }
}