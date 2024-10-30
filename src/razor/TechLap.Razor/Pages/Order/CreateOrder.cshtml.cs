using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TechLap.API;
using TechLap.API.DTOs.Requests;
using CustomerResponse = TechLap.API.DTOs.Responses.CustomerDTOs.CustomerResponse;
using ProductResponse = TechLap.API.DTOs.Responses.ProductDTOs.ProductResponse;

namespace TechLap.Razor.Pages.Order
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CreateModel> _logger;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public required OrderRequest OrderRequest { get; set; }
        [BindProperty]
        public List<OrderDetailRequest> OrderDetailRequest { get; set; } = new List<OrderDetailRequest>();
        public List<ProductResponse>? Products { get; set; } = new List<ProductResponse>();
        public List<CustomerResponse>? Customers { get; set; } = new List<CustomerResponse>();

        public CreateModel(ILogger<CreateModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!await IsAuthorizedAsync())
            {
                Response.Cookies.Delete("AuthToken");
                return RedirectToPage("/Login/Index");
            }

            Products = await LoadDataAsync<ProductResponse>("api/products");
            Customers = await LoadDataAsync<CustomerResponse>("api/customers");

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

        private async Task<List<T>?> LoadDataAsync<T>(string endpoint)
        {
            var token = Request.Cookies["AuthToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            string? apiEndpoint = _configuration["ApiEndPoint"];

            var response = await client.GetAsync($"{apiEndpoint}/{endpoint}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<T>>>(responseBody);

                return apiResponse?.Data;
            }
            else
            {
                _logger.LogError("API call to {Endpoint} failed with status code: {StatusCode}", endpoint, response.StatusCode);
                return null;
            }
        }
    }
}
