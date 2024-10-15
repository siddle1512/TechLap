using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.ProductDTOs;
using TechLap.API.Mapper;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : BaseController<ProductController>
    {
        private IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet] 
        public async Task<IActionResult> GetProducts([FromQuery] ProductRequest request)
        {
            var products = await _productRepository.GetAllAsync(p => p.Model.ToLower().Contains(request.Model.ToLower()));
            var response = LazyMapper.Mapper.Map<IEnumerable<ProductResponse>>(products);
            return CreateResponse<IEnumerable<ProductResponse>>(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }
        
        [HttpPost] 
        [Route("api/product/search")]
        public async Task<IActionResult> SearchProducts([FromBody] SearchProductsRequest request)
        {
           var products = await _productRepository.SearchProductsAsync(request);
           return CreateResponse(true, "Request processed successfully.", HttpStatusCode.OK, products);
        }
    }
}
