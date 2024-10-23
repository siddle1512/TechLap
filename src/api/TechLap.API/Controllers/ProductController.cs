using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.ProductDTOs;
using TechLap.API.Exceptions;
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
        [Authorize(Roles = "User, Admin")]
        [Route("/api/products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync(p => true);
            var response = LazyMapper.Mapper.Map<IEnumerable<ProductResponse>>(products);
            return CreateResponse<IEnumerable<ProductResponse>>(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }

        //[HttpGet]
        //[Authorize(Roles = "User, Admin")]
        //[Route("/api/products/")]
        //public async Task<IActionResult> GetProducts([FromQuery] SearchProductRequestByModel request)
        //{
        //    var products = await _productRepository.GetAllAsync(p => p.Model.ToLower().Contains(request.Model.ToLower()));
        //    var response = LazyMapper.Mapper.Map<IEnumerable<ProductResponse>>(products);
        //    return CreateResponse<IEnumerable<ProductResponse>>(true, "Request processed successfully.", HttpStatusCode.OK, response);
        //}

        [HttpPost]
        [Authorize(Roles = "User, Admin")]
        [Route("/api/products")]
        public async Task<IActionResult> AddProduct([FromBody] ProductRequest request)
        {
            var product = LazyMapper.Mapper.Map<Product>(request);
            var addedProduct = await _productRepository.AddAsync(product);

            var response = LazyMapper.Mapper.Map<ProductResponse>(addedProduct);
            return CreateResponse<int>(true, "Product added successfully.", HttpStatusCode.Created, response.Id);
        }

        [HttpPut]
        [Authorize(Roles = "User, Admin")]
        [Route("/api/products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequest request)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new NotFoundException($"Product with ID {id} does not exist.");
            }

            var updatedProduct = LazyMapper.Mapper.Map(request, existingProduct);

            await _productRepository.UpdateAsync(updatedProduct);

            var response = LazyMapper.Mapper.Map<ProductResponse>(updatedProduct);
            return CreateResponse<int>(true, "Product updated successfully.", HttpStatusCode.OK, response.Id);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                throw new NotFoundException($"Product with ID {id} does not exist.");
            }

            await _productRepository.DeleteAsync(existingProduct);

            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK);
        }
    }
}
