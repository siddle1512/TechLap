using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechLap.API.DTOs.Requests;
using TechLap.API.Exceptions;
using TechLap.API.Mapper;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Controllers
{
    public class CategoryController : BaseController<CategoryController>
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        [HttpGet]
        [Route("/api/categories")]
        public async Task<IActionResult> GetCategors()
        {
            var categoryList = await _categoryRepository.GetAllAsync(o => true);
            return CreateResponse<IEnumerable<Category>>(true, "Request processed successfully.", HttpStatusCode.OK, categoryList);
        }

        [HttpGet]
        [Route("/api/categories/{id:int}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return CreateResponse<Category>(true, "Request processed successfully.", HttpStatusCode.OK, category);
        }

        [HttpPost]
        [Route("/api/categories")]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
        {
            var category = LazyMapper.Mapper.Map<CreateCategoryRequest, Category>(request);
            category = await _categoryRepository.AddAsync(category);
            return CreateResponse<int>(true, "Request processed successfully.", HttpStatusCode.OK, category.Id);
        }

        [HttpPut]
        [Route("/api/categories/{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, CreateCategoryRequest request)
        {
            if (id != request.Id)
            {
                throw new BadRequestException("Id do not match");
            }
            var category = LazyMapper.Mapper.Map<CreateCategoryRequest, Category>(request);
            await _categoryRepository.UpdateAsync(category);
            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK, "CategoryId " + id + " updated.");
        }

        [HttpDelete]
        [Route("/api/categories/{id:int}")]
        public async Task<IActionResult> RemoveCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new NotFoundException($"Category with ID {id} does not exist.");
            }
            await _categoryRepository.DeleteAsync(category);
            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK, "CategoryId " + id + " removed.");
        }
    }
}
