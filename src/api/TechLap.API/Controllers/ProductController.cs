using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.ProductDTOs;
using TechLap.API.Mapper;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Controllers
{
    public class ProductController : BaseController<ProductController>
    {
        private IProductRepository _productRepository;
        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        [Route("/api/products")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductRequest request)
        {
            var products = await _productRepository.GetAllAsync(p => p.Model.ToLower().Contains(request.model.ToLower()));
            var response = LazyMapper.Mapper.Map<IEnumerable<ProductResponse>>(products);
            return CreateResponse<IEnumerable<ProductResponse>>(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }
    }
}
