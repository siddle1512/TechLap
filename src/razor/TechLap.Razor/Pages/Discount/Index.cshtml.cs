using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TechLap.API;
using TechLap.API.DTOs.Requests.DiscountRequests;
using TechLap.API.DTOs.Responses.DiscountRespones;
using TechLap.API.Enums;

namespace TechLap.Razor.Pages.Discount;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IndexModel> _logger;
    private readonly IConfiguration _configuration;
    public List<GetAdminDiscountRespones>? Discounts { get; set; }
    [BindProperty] public AddAdminDiscountRequest NewDiscount { get; set; }
    [BindProperty] public UpdateAdminDiscountRequest UpdateDiscount { get; set; }
    public IEnumerable<SelectListItem> StatusOptions { get; set; }


    public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        NewDiscount = new AddAdminDiscountRequest("", 0, DateTime.Now, DateTime.Now, 0, 0);
    }

    public async Task<IActionResult> OnGet()
    {
        
        StatusOptions = Enum.GetValues(typeof(DiscountStatus))
            .Cast<DiscountStatus>()
            .Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(), // Giá trị sẽ được gửi
                Text = e.ToString() // Hiển thị tên enum
            });
        
        if (!await IsAuthorizedAsync())
        {
            Response.Cookies.Delete("AuthToken"); // Xoá token nếu không hợp lệ
            return RedirectToPage("/Login/Index"); // Chuyển hướng đến trang đăng nhập
        }

        Discounts = await LoadDiscountAsync();

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

    private async Task<List<GetAdminDiscountRespones>?> LoadDiscountAsync()
    {
        var token = Request.Cookies["AuthToken"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        string? apiEndpoint = _configuration["ApiEndPoint"];

        var response = await client.GetAsync($"{apiEndpoint}/api/discounts");

        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<GetAdminDiscountRespones>>>(responseBody);

            return apiResponse?.Data;
        }
        else
        {
            _logger.LogError("API call failed with status code: {StatusCode}", response.StatusCode);
            return null;
        }
    }

    public async Task<IActionResult> OnPostAddDiscountAsync()
    {
        if (!await IsAuthorizedAsync())
        {
            return RedirectToPage("/Login/Index");
        }

        
        var errorMessages = new List<string>();
        if (errorMessages.Any())
        {
            TempData["ErrorMessages"] = errorMessages;
            Discounts = await LoadDiscountAsync();
            return Page();
        }

        // Kiểm tra điều kiện của Discount Code
        if (NewDiscount.DiscountCode.Length < 4)
        {
           throw new Exception("Discount code phải có ít nhất 4 ký tự.");
        }
        
        // Kiểm tra điều kiện của Discount Percentage
        if (NewDiscount.DiscountPercentage < 1 || NewDiscount.DiscountPercentage > 100)
        {
            throw new Exception("Discount percentage phải trong khoảng từ 1 đến 100%.");
        }
        
        // Kiểm tra điều kiện của Start Date và End Date
        if (NewDiscount.StartDate < DateTime.Today)
        {
            throw new Exception("Start Date phải là ngày hôm nay hoặc sau đó.");
        }
        
        if (NewDiscount.StartDate >= NewDiscount.EndDate)
        {
            throw new Exception("End Date phải lớn hơn Start Date.");
        }
        
        // Nếu có lỗi, lưu vào TempData và hiển thị dialog lỗi
        

        var token = Request.Cookies["AuthToken"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        string? apiEndpoint = _configuration["ApiEndPoint"];

        var jsonContent = JsonConvert.SerializeObject(NewDiscount);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{apiEndpoint}/api/discounts/create", content);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Discount added successfully.");

            // Sử dụng TempData để lưu thông báo thành công
            TempData["SuccessMessage"] = "Discount added successfully!";

            // Khởi tạo lại NewDiscount với giá trị mặc định
            NewDiscount = new AddAdminDiscountRequest("", 0, DateTime.Now, DateTime.Now, 0, 0);

            return RedirectToPage();
        }
        else
        {
            _logger.LogError("Failed to add discount with status code: {StatusCode}", response.StatusCode);
            TempData["ErrorMessages"] = new List<string> { "Error adding discount." };
        }

        Discounts = await LoadDiscountAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostEditDiscountAsync(int id)
    {
        if (!await IsAuthorizedAsync())
        {
            return RedirectToPage("/Login/Index");
        }

        var token = Request.Cookies["AuthToken"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        string? apiEndpoint = _configuration["ApiEndPoint"];

        // Chuyển đối tượng cập nhật thành JSON
        var updateDiscountRequest = new UpdateAdminDiscountRequest(
            UpdateDiscount.DiscountCode,
            UpdateDiscount.DiscountPercentage,
            UpdateDiscount.StartDate,
            UpdateDiscount.EndDate,
            UpdateDiscount.UsageLimit,
            UpdateDiscount.Status
        );
        var content = new StringContent(JsonConvert.SerializeObject(updateDiscountRequest), Encoding.UTF8,
            "application/json");

        // Gọi PUT API với dữ liệu cập nhật
        var response = await client.PutAsync($"{apiEndpoint}/api/discounts/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Discount updated successfully.");
            TempData["SuccessMessage"] = "Discount updated successfully!";
            return RedirectToPage();
        }
        else
        {
            _logger.LogError("Failed to update discount with status code: {StatusCode}", response.StatusCode);
            TempData["ErrorMessages"] = new List<string> { "Error updating discount." };
        }

        Discounts = await LoadDiscountAsync();
        return Page();
    }


    public async Task<IActionResult> OnPostDeleteDiscountAsync(int id)
    {
        if (!await IsAuthorizedAsync())
        {
            return RedirectToPage("/Login/Index");
        }

        var token = Request.Cookies["AuthToken"];
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        string? apiEndpoint = _configuration["ApiEndPoint"];

        var response = await client.DeleteAsync($"{apiEndpoint}/api/discounts/delete");

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Discount deleted successfully.");
            TempData["SuccessMessage"] = "Discount deleted successfully!";
        }
        else
        {
            _logger.LogError("Failed to delete discount with status code: {StatusCode}", response.StatusCode);
            TempData["ErrorMessages"] = new List<string> { "Error deleting discount." };
        }

        return RedirectToPage();
    }
}