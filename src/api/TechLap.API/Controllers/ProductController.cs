using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> GetProducts([FromQuery] ProductRequest request)
        {
            var products = await _productRepository.GetAllAsync(p => p.Model.ToLower().Contains(request.Model.ToLower()));
            var response = LazyMapper.Mapper.Map<IEnumerable<ProductResponse>>(products);
            return CreateResponse<IEnumerable<ProductResponse>>(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("/api/products")]
        public async Task<IActionResult> AddProduct([FromBody] ProductRequest request)
        {
            try
            {
                // Check if CategoryId exists in the Categories table
                var isCategoryValid = await _productRepository.IsCategoryValidAsync(request.CategoryId);
                if (!isCategoryValid)
                {
                    throw new BadRequestException($"Category with ID {request.CategoryId} does not exist.");
                }

                // If CategoryId is valid, proceed to add product
                var product = LazyMapper.Mapper.Map<Product>(request);
                var addedProduct = await _productRepository.AddAsync(product);

                var response = LazyMapper.Mapper.Map<ProductResponse>(addedProduct);
                return CreateResponse(true, "Product added successfully.", HttpStatusCode.Created, response);
            }
            catch (DbUpdateException ex)
            {
                return CreateResponse<ProductResponse>(false, $"Error while adding product: {ex.InnerException?.Message ?? ex.Message}", HttpStatusCode.InternalServerError);
            }
            catch (Exception ex)
            {
                return CreateResponse(false, "An error occurred while adding the product.", HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        // PUT: api/products/{id}
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("/api/products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRequest request)
        {
                // Ensure the request model is valid
                if (!ModelState.IsValid)
                {
                    throw new BadRequestException("Invalid request data.");
                }

            // Fetch the existing product by ID
            var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    throw new NotFoundException($"Product with ID {id} does not exist.");
                }

            // Ensure the CategoryId exists             
            var categoryExists = await _productRepository.IsCategoryValidAsync(request.CategoryId);
                if (!categoryExists)
                {
                    throw new BadRequestException($"Category with ID {request.CategoryId} does not exist.");
                }

                // Map request data to the existing product
                var updatedProduct = LazyMapper.Mapper.Map(request, existingProduct);

                // Update the product
                await _productRepository.UpdateAsync(updatedProduct);

                // Map the updated product to the response model
                var response = LazyMapper.Mapper.Map<ProductResponse>(updatedProduct);
                return CreateResponse(true, "Product updated successfully.", HttpStatusCode.OK, response);
        }


        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
                // Check if the product exists
                var existingProduct = await _productRepository.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    throw new NotFoundException($"Product with ID {id} does not exist.");
                }

                // Delete the product
                await _productRepository.DeleteAsync(existingProduct);

                // Optionally, return a success message
                return Ok(new { message = "Product deleted successfully." });
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
