using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TechLap.API;
using TechLap.API.DTOs.Responses.ProductDTOs;
using TechLap.API.Models;

namespace TechLap.Razor.Pages.Product
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public ProductResponse Product { get; set; } = new ProductResponse();
        public List<Category> Categories { get; set; } = new List<Category>();

        public EditModel(ILogger<IndexModel> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet(int id)
        {
            Categories = await LoadCategoriesAsync();
            Product = await GetProductById(id);
            return Page();
        }

        public async Task<List<Category>> LoadCategoriesAsync()
        {
            var token = Request.Cookies["AuthToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string apiEndpoint = $"{_configuration["ApiEndPoint"]}/api/categories/";

            try
            {
                var response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<Category>>>(responseBody);
                    return apiResponse?.Data ?? new List<Category>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while loading categories.");
            }
            return new List<Category>();
        }

        public async Task<ProductResponse> GetProductById(int id)
        {
            var token = Request.Cookies["AuthToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string apiEndpoint = $"{_configuration["ApiEndPoint"]}/api/products/" + id;
            try
            {
                var response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<ProductResponse>>(responseBody);
                    return apiResponse?.Data ?? new ProductResponse();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while loading categories.");
            }
            return new ProductResponse(); ;
        }

        public async Task<IActionResult> OnPostEdit()
        {
            var token = Request.Cookies["AuthToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string apiEndpoint = $"{_configuration["ApiEndPoint"]}/api/products/" + Product.Id;

            try
            {
                var jsonContent = JsonConvert.SerializeObject(Product);
                var response = await client.PutAsync(apiEndpoint, new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to update product with ID {Id}. Status code: {StatusCode}", Product.Id, response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error updating product.");
            }

            return RedirectToPage("/Product/Index");
        }
    }
}
