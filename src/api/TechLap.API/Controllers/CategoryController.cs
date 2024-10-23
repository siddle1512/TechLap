using Microsoft.AspNetCore.Mvc;
using System.Net;
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
    }
}
