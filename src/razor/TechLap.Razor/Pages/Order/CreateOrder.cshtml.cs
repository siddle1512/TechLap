using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TechLap.API;
using TechLap.API.DTOs.Requests;
using CustomerResponse = TechLap.API.DTOs.Responses.CustomerDTOs.CustomerResponse;

namespace TechLap.Razor.Pages.Order
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CreateModel> _logger;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public OrderRequest OrderRequest { get; set; } = default!;
        [BindProperty]
        public List<OrderDetailRequest> OrderDetailRequest { get; set; } = new List<OrderDetailRequest>();
        public List<CustomerResponse>? Customers { get; set; } = new List<CustomerResponse>();
        public string Token { get; set; } = string.Empty;

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
            Token = Request.Cookies["AuthToken"];
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

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                OrderDetailRequest ??= new List<OrderDetailRequest>();

                if (OrderRequest.CustomerId == null || OrderRequest.CustomerId == 0)
                {
                    ModelState.AddModelError("OrderRequest.CustomerId", "Please select a customer");
                }

                if (OrderDetailRequest == null || !OrderDetailRequest.Any())
                {
                    ModelState.AddModelError("OrderDetailRequest", "Please add at least one product");
                }

                OrderDetailRequest = OrderDetailRequest
                    .Where(detail => detail.ProductId != 0 && detail.Quantity > 0)
                    .ToList();

                var totalPrice = CalculateTotalPrice();

                var orderDate = OrderRequest.OrderDate == default ? DateTime.Now : OrderRequest.OrderDate;

                var newOrderRequest = new OrderRequest(
                    OrderRequest.OrderDate,
                    totalPrice,
                    OrderRequest.Payment,
                    OrderRequest.Status,
                    OrderRequest.DiscountId,
                    OrderDetailRequest,
                    OrderRequest.CustomerId
                );

                var token = Request.Cookies["AuthToken"];
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                string? apiEndpoint = _configuration["ApiEndPoint"];

                var response = await client.PostAsJsonAsync($"{apiEndpoint}/api/orders", newOrderRequest);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Order/Index");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error occurred while adding the order: {errorContent}");

                Customers = await LoadDataAsync<CustomerResponse>("api/customers");
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while processing your request.");

                Customers = await LoadDataAsync<CustomerResponse>("api/customers");
                return Page();
            }
        }

        private decimal CalculateTotalPrice()
        {
            if (OrderDetailRequest == null || !OrderDetailRequest.Any())
                return 0;

            return OrderDetailRequest
                .Where(detail => detail.ProductId != 0 && detail.Quantity > 0)
                .Sum(detail => detail.Price * detail.Quantity);
        }
    }
}
