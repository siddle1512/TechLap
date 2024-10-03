using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TechLap.API.Responses;

namespace TechLap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public LoginResponse Post([FromBody] LoginRequest value)
        {
            return new LoginResponse();
        }
    }
}
