using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TechLap.Razor.Pages.Logout
{
    public class IndexModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            HttpContext.Response.Cookies.Delete("AuthToken");

            return RedirectToPage("/Index");
        }

    }
}
