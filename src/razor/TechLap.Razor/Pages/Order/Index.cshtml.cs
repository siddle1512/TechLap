using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TechLap.API;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.OrderDTOs;
using TechLap.API.Enums;
using CustomerResponse = TechLap.API.DTOs.Responses.CustomerDTOs.CustomerResponse;
using ProductResponse = TechLap.API.DTOs.Responses.ProductDTOs.ProductResponse;

namespace TechLap.Razor.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public List<OrderResponse>? Orders { get; set; }
        public List<ProductResponse>? Products { get; set; }
        public List<CustomerResponse>? Customers { get; set; }
        [BindProperty]
        public OrderRequest? OrderRequest { get; set; }

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

            Orders = await LoadDataAsync<OrderResponse>("api/orders");
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

        public async Task<IActionResult> OnPostCreateOrderAsync()
        {
            // Kiểm tra xác thực token
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Token is missing.");
                return Unauthorized();
            }

            // Kiểm tra OrderRequest
            if (OrderRequest == null || OrderRequest.OrderDetails == null || !OrderRequest.OrderDetails.Any())
            {
                return BadRequest(new { success = false, message = "Order details are missing." });
            }

            // Tạo đối tượng OrderRequest mới
            var orderRequest = new OrderRequest(
                OrderDate: DateTime.UtcNow,
                TotalPrice: OrderRequest.TotalPrice,
                Payment: Enum.TryParse(OrderRequest.Payment.ToString(), out PaymentMethod paymentMethod) ? paymentMethod : default,
                Status: Enum.TryParse(OrderRequest.Status.ToString(), out OrderStatus status) ? status : default,
                DiscountId: OrderRequest.DiscountId,
                OrderDetails: OrderRequest.OrderDetails,
                CustomerId: OrderRequest.CustomerId
            );

            // Tạo client HTTP
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiEndpoint = _configuration["ApiEndPoint"];

            // Chuyển đổi OrderRequest thành JSON
            var content = new StringContent(JsonConvert.SerializeObject(orderRequest), System.Text.Encoding.UTF8, "application/json");

            try
            {
                // Gửi yêu cầu POST đến API
                var response = await client.PostAsync($"{apiEndpoint}/api/orders", content);

                // Xử lý phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    return new JsonResult(new { success = true, message = "Order created successfully!" });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to create order: {ErrorContent}", errorContent);
                    return new JsonResult(new { success = false, message = "Failed to create order." });
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order.");
                return new JsonResult(new { success = false, message = "An error occurred while creating the order." });
            }
        }
    }
}
