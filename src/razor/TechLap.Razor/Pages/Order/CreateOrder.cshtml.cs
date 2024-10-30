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
        public OrderRequest OrderRequest { get; set; } = default!;
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

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Debug logging
                _logger.LogInformation($"Received OrderRequest: {JsonConvert.SerializeObject(OrderRequest)}");
                _logger.LogInformation($"Received OrderDetailRequest: {JsonConvert.SerializeObject(OrderDetailRequest)}");

                // Initialize OrderDetailRequest if null
                OrderDetailRequest ??= new List<OrderDetailRequest>();

                // Validate required fields
                if (OrderRequest.CustomerId == null || OrderRequest.CustomerId == 0)
                {
                    ModelState.AddModelError("OrderRequest.CustomerId", "Please select a customer");
                }

                if (OrderDetailRequest == null || !OrderDetailRequest.Any())
                {
                    ModelState.AddModelError("OrderDetailRequest", "Please add at least one product");
                }

                //if (!ModelState.IsValid)
                //{
                //    // Reload the dropdown data before returning the page
                //    Products = await LoadDataAsync<ProductResponse>("api/products");
                //    Customers = await LoadDataAsync<CustomerResponse>("api/customers");
                //    return Page();
                //}

                // Clean up OrderDetailRequest - remove any invalid entries
                OrderDetailRequest = OrderDetailRequest
                    .Where(detail => detail.ProductId != 0 && detail.Quantity > 0)
                    .ToList();

                // Calculate total price
                var totalPrice = CalculateTotalPrice();

                // Set order date if not already set
                var orderDate = OrderRequest.OrderDate == default ? DateTime.Now : OrderRequest.OrderDate;

                // Create order request
                var newOrderRequest = new OrderRequest(
                    OrderRequest.OrderDate,
                    totalPrice,
                    OrderRequest.Payment,
                    OrderRequest.Status,
                    OrderRequest.DiscountId,
                    OrderDetailRequest, // Ensure OrderDetailRequest is not null
                    OrderRequest.CustomerId
                );

                // Log the request being sent to API
                _logger.LogInformation($"Sending order request to API: {JsonConvert.SerializeObject(newOrderRequest)}");

                // Call API to create order
                var token = Request.Cookies["AuthToken"];
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                string? apiEndpoint = _configuration["ApiEndPoint"];

                var response = await client.PostAsJsonAsync($"{apiEndpoint}/api/orders", newOrderRequest);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Order/Index");
                }

                // If API call fails, add error and show it to user
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"API Error: {errorContent}");
                ModelState.AddModelError(string.Empty, $"Error occurred while adding the order: {errorContent}");

                // Reload the dropdown data
                Products = await LoadDataAsync<ProductResponse>("api/products");
                Customers = await LoadDataAsync<CustomerResponse>("api/customers");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing order creation");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred while processing your request.");

                // Reload the dropdown data
                Products = await LoadDataAsync<ProductResponse>("api/products");
                Customers = await LoadDataAsync<CustomerResponse>("api/customers");
                return Page();
            }
        }


        // Method to calculate the total price from OrderDetailRequest
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
