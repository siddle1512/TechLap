using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using TechLap.API.Hubs;

namespace TechLap.Razor.Pages.Chat
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ChatHub> _hubContext;

        public IndexModel(IHubContext<ChatHub> hubContext, IConfiguration configuration)
        {
            _hubContext = hubContext;
            _configuration = configuration;
        }


        public string UserId { get; set; }
        public string ApiEndpoint { get; set; }
        public IActionResult OnGet()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                UserId = jwtToken.Claims.First(claim => claim.Type == "id").Value;
            }
            ApiEndpoint = _configuration["ApiEndpoint"];
            return Page();
        }
    }
}
