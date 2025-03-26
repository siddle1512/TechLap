using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TechLap.API;
using TechLap.API.DTOs.Requests.DiscountRequests;
using TechLap.API.DTOs.Responses.DiscountRespones;
using TechLap.API.Models;

namespace TechLap.Razor.Pages.Discount;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _configuration;
    public List<API.Models.Discount> Discounts { get; set; }
    [BindProperty] public AddAdminDiscountRequest NewDiscount { get; set; }
    [BindProperty] public UpdateAdminDiscountRequest UpdateDiscount { get; set; }
    public IEnumerable<SelectListItem> StatusOptions { get; set; }
    public string? ErrorMessage { get; set; }
    
    public string ApiEndpoint { get; private set; } = string.Empty;
    public string AuthToken { get; private set; } = string.Empty;


    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        NewDiscount = new AddAdminDiscountRequest("", 0, DateTime.Now, DateTime.Now, 0, 0);
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

        Discounts = await LoadDiscountAsync(token);

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

    private async Task<List<API.Models.Discount>> LoadDiscountAsync(string token)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        string apiEndpoint = $"{_configuration["ApiEndPoint"]}/api/discounts/";
        try
        {
            var response = await client.GetAsync(apiEndpoint);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<API.Models.Discount>>>(responseBody);
                return apiResponse?.Data ?? new List<API.Models.Discount>();
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "An error occurred while loading categories.");
            ErrorMessage = "Failed to load categories.";
        }
        return new List<API.Models.Discount>();
    }
    
//     public async Task<IActionResult> OnGetDiscountsAsync()
//     {
//         var discounts = await LoadDiscountAsync();
//         return new JsonResult(discounts);
//     }
//
//     public async Task<IActionResult> OnPostAddDiscountAsync()
//     {
//         if (!await IsAuthorizedAsync())
//         {
//             return RedirectToPage("/Login/Index");
//         }
//
//         var token = Request.Cookies["AuthToken"];
//         var client = _httpClientFactory.CreateClient();
//         client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
//         string? apiEndpoint = _configuration["ApiEndPoint"];
//
//         var jsonContent = JsonConvert.SerializeObject(NewDiscount);
//         var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
//
//         var response = await client.PostAsync($"{apiEndpoint}/api/discounts/create", content);
//
//         if (response.IsSuccessStatusCode)
//         {
//             _logger.LogInformation("Discount added successfully.");
//
//             // Sử dụng TempData để lưu thông báo thành công
//             TempData["SuccessMessage"] = "Discount added successfully!";
//
//             // Khởi tạo lại NewDiscount với giá trị mặc định
//             NewDiscount = new AddAdminDiscountRequest("", 0, DateTime.Now, DateTime.Now, 0, 0);
//
//             return RedirectToPage();
//         }
//         else
//         {
//             _logger.LogError("Failed to add discount with status code: {StatusCode}", response.StatusCode);
//             TempData["ErrorMessages"] = new List<string> { "Error adding discount." };
//         }
//
//         Discounts = await LoadDiscountAsync();
//         return Page();
//     }
//
//     public async Task<IActionResult> OnPostEditDiscountAsync(int id)
// {
//     if (!await IsAuthorizedAsync())
//     {
//         return RedirectToPage("/Login/Index");
//     }
//
//     var token = Request.Cookies["AuthToken"];
//     var client = _httpClientFactory.CreateClient();
//     client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
//     string? apiEndpoint = _configuration["ApiEndPoint"];
//
//     var updateDiscount = new UpdateAdminDiscountRequest(
//         UpdateDiscount.DiscountCode,
//         UpdateDiscount.DiscountPercentage,
//         UpdateDiscount.StartDate,
//         UpdateDiscount.EndDate,
//         UpdateDiscount.UsageLimit,
//         UpdateDiscount.Status
//     );
//
//     var jsonContent = JsonConvert.SerializeObject(updateDiscount);
//     var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
//
//     var response = await client.PutAsync($"{apiEndpoint}/api/discounts/{id}", content);
//
//     if (response.IsSuccessStatusCode)
//     {
//         _logger.LogInformation("Discount updated successfully.");
//         TempData["SuccessMessage"] = "Discount updated successfully!";
//         return RedirectToPage();
//     }
//     else
//     {
//         var errorContent = await response.Content.ReadAsStringAsync();
//         _logger.LogError("Failed to update discount with status code: {StatusCode}, Error: {ErrorContent}", response.StatusCode, errorContent);
//         TempData["ErrorMessages"] = new List<string> { "Error updating discount." };
//     }
//
//     Discounts = await LoadDiscountAsync();
//     return Page();
// }
//
//
//
//
//     public async Task<IActionResult> OnPostDeleteDiscountAsync(int id)
//     {
//         if (!await IsAuthorizedAsync())
//         {
//             return RedirectToPage("/Login/Index");
//         }
//
//         var token = Request.Cookies["AuthToken"];
//         var client = _httpClientFactory.CreateClient();
//         client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
//         string? apiEndpoint = _configuration["ApiEndPoint"];
//
//         var response = await client.DeleteAsync($"{apiEndpoint}/api/discounts/{id}");
//
//         if (response.IsSuccessStatusCode)
//         {
//             _logger.LogInformation("Discount deleted successfully.");
//             TempData["SuccessMessage"] = "Discount deleted successfully!";
//         }
//         else
//         {
//             _logger.LogError("Failed to delete discount with status code: {StatusCode}", response.StatusCode);
//             TempData["ErrorMessages"] = new List<string> { "Error deleting discount." };
//         }
//
//         return RedirectToPage();
//     }
}