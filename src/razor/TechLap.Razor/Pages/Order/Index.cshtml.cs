using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TechLap.API;
using TechLap.API.DTOs.Responses.OrderDTOs;

namespace TechLap.Razor.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        public List<OrderResponse>? Orders { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            if (!await IsAuthorizedAsync())
            {
                Response.Cookies.Delete("AuthToken");  // Xoá token nếu không hợp lệ
                return RedirectToPage("/Login/Index"); // Chuyển hướng đến trang đăng nhập
            }

            Orders = await LoadOrdersAsync();

            return Page();
        }

        /// <summary>
        /// Kiểm tra xem người dùng có quyền truy cập vào trang này không
        /// </summary>
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

        private async Task<List<OrderResponse>?> LoadOrdersAsync()
        {
            var token = Request.Cookies["AuthToken"];
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            string? apiEndpoint = _configuration["ApiEndPoint"];

            var response = await client.GetAsync($"{apiEndpoint}/api/orders");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<OrderResponse>>>(responseBody);

                return apiResponse?.Data;
            }
            else
            {
                _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);
                return null;
            }
        }
    }
}
