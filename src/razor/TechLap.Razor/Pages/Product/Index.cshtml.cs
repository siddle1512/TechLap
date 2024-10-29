using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TechLap.API;
using TechLap.API.DTOs.Responses.ProductDTOs;
using System.Net.Http.Headers;
using TechLap.API.Models;

namespace TechLap.Razor.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public ProductResponse Product { get; set; }
        public List<ProductResponse>? Products { get; set; }
        public string? ErrorMessage { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public List<Category> Categories { get; set; } = new List<Category>();
        public async Task<IActionResult> OnGet()
        {
            if (!await IsAuthorizedAsync())
            {
                Response.Cookies.Delete("AuthToken");
                return RedirectToPage("/Login/Index");
            }

            Products = await LoadProductsAsync();
            Categories = await LoadCategoriesAsync();
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
        public async Task<IActionResult> OnPost(ProductResponse newProduct)
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Invalid product data.";
                return Page();
            }

            

            var token = Request.Cookies["AuthToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string apiEndpoint = $"{_configuration["ApiEndPoint"]}/api/products";

            try
            {
                var jsonContent = JsonConvert.SerializeObject(newProduct);
                var response = await client.PostAsync(apiEndpoint, new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage();
                }

                ErrorMessage = "Failed to add product.";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error adding product.");
                ErrorMessage = "An error occurred. Please try again later.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostEdit(ProductResponse updatedProduct)
        {
            _logger.LogInformation("B?t ð?u th?c hi?n c?p nh?t s?n ph?m v?i ID: {Id}", updatedProduct.Id);
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Invalid product data.";
                _logger.LogWarning("ModelState không h?p l? khi c?p nh?t s?n ph?m v?i ID: {Id}", updatedProduct.Id);
                RedirectToPage("/Product");
            }

            var token = Request.Cookies["AuthToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string apiEndpoint = $"{_configuration["ApiEndPoint"]}/api/products/{updatedProduct.Id}";
            _logger.LogInformation("API endpoint cho c?p nh?t s?n ph?m: {ApiEndpoint}", apiEndpoint);

            try
            {
                var jsonContent = JsonConvert.SerializeObject(updatedProduct);
                _logger.LogInformation("D? li?u JSON cho s?n ph?m c?p nh?t: {JsonContent}", jsonContent);
                var response = await client.PutAsync(apiEndpoint, new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("C?p nh?t s?n ph?m thành công v?i ID: {Id}", updatedProduct.Id);
                    RedirectToPage("/Product"); 
                }
                _logger.LogWarning("Yêu c?u PUT th?t b?i v?i m? tr?ng thái: {StatusCode}", response.StatusCode);
                ErrorMessage = "Failed to update product.";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error updating product.");
                ErrorMessage = "An error occurred. Please try again later.";
            }

            _logger.LogInformation("Hoàn t?t phýõng th?c OnPostEdit v?i ID s?n ph?m: {Id}", updatedProduct.Id);
            return Page();
        }
         
        public async Task<IActionResult> OnGetDelete(int? id)
        {
            var token = Request.Cookies["AuthToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string apiEndpoint = $"{_configuration["ApiEndPoint"]}/api/products/{id}";

            try
            {
                var response = await client.DeleteAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    Products = await LoadProductsAsync();
                    return RedirectToPage(); 
                }

                ErrorMessage = "Failed to delete product.";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error deleting product.");
                ErrorMessage = "An error occurred. Please try again later.";
            }
            return Page();
        }

        private async Task<bool> IsAuthorizedAsync()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing in the request.");
                return false;
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string? apiEndpoint = _configuration["ApiEndPoint"];

            try
            {
                var response = await client.GetAsync($"{apiEndpoint}/api/user/validateToken");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("Unauthorized access attempted with expired/invalid token.");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while validating the token.");
            }

            return false;
        }

        private async Task<List<ProductResponse>?> LoadProductsAsync()
        {
            var token = Request.Cookies["AuthToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string? apiEndpoint = _configuration["ApiEndPoint"];

            try
            {
                var response = await client.GetAsync($"{apiEndpoint}/api/products");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<ProductResponse>>>(responseBody);
                    return apiResponse?.Data;
                }
                else
                {
                    _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);
                    ErrorMessage = "Failed to load products. Please try again later.";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while loading products.");
                ErrorMessage = "An error occurred. Please try again later.";
            }

            return null;
        }
    }
}
