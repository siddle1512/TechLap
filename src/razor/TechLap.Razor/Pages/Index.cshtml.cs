using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TechLap.Razor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGet()
        {
            var token = Request.Cookies["AuthToken"];

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            string apiEndpoint = _configuration["ApiEndPoint"];
            var response = await client.GetAsync(apiEndpoint + "/api/user/validateToken");

            if (response.IsSuccessStatusCode)
            {
                return Page();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Response.Cookies.Delete("AuthToken");
            }

            return Page();
        }
    }
}
