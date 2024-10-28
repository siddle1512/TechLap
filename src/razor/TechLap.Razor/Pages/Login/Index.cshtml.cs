using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TechLap.Razor.Pages.Login
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string Message { get; set; }

        private readonly IConfiguration _configuration;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {

                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    Message = "Username and Password cannot be empty";
                    return Page();
                }

                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = new StringContent(
                    JsonConvert.SerializeObject(new { Email = Username, Password = Password }),
                    Encoding.UTF8,
                    "application/json"
                );

                string apiEndpoint = _configuration["ApiEndPoint"];
                var response = await client.PostAsync(apiEndpoint + "/api/user/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JObject.Parse(responseBody);

                    if (jsonResponse["isSuccess"] != null && jsonResponse["isSuccess"].Value<bool>())
                    {
                        var token = jsonResponse["data"].Value<string>();

                        HttpContext.Session.SetString("Token", token);
                        HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
                        {
                            HttpOnly = true,
                            Expires = DateTimeOffset.UtcNow.AddHours(1)
                        });
                        TempData["Token"] = token;
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        Message = "Login failed: " + jsonResponse["message"]?.ToString();
                    }
                }
                else
                {
                    Message = "API call failed with status code: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                Message = "An error occurred: " + ex.Message;
            }
            finally
            {
            }

            return Page();
        }
    }
}
