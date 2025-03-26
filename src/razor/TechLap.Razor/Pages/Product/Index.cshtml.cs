using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TechLap.API;
using TechLap.API.Models;

namespace TechLap.Razor.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public string ApiEndpoint { get; private set; } = string.Empty;
        public string AuthToken { get; private set; } = string.Empty;
        public List<Category> Categories { get; set; } = new List<Category>();
        public string? ErrorMessage { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                Response.Cookies.Delete("AuthToken");
                return RedirectToPage("/Login/Index");
            }

            if (!await IsAuthorizedAsync(token))
            {
                Response.Cookies.Delete("AuthToken");
                return RedirectToPage("/Login/Index");
            }

            AuthToken = token;
            ApiEndpoint = _configuration["ApiEndPoint"] ?? string.Empty;
            Categories = await LoadCategoriesAsync(token);

            return Page();
        }

        private async Task<bool> IsAuthorizedAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing in the request.");
                return false;
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
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

        private async Task<List<Category>> LoadCategoriesAsync(string token)
        {
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
                ErrorMessage = "Failed to load categories.";
            }
            return new List<Category>();
        }
    }
}