using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TechLap.API.Configurations;

namespace TechLap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase where T : BaseController<T>
    {
        protected IActionResult CreateResponse<Response>(bool isSuccess, string message, HttpStatusCode statusCode, Response? data = default)
        {
            var apiResponse = new ApiResponse<Response>
            {
                IsSuccess = isSuccess,
                Data = data,
                Message = "Request processed successfully."
            };
            return StatusCode((int)statusCode, apiResponse);
        }

        protected string GenerateJwtToken(string id, string role)
        {
            var issuer = JwtConfig._configuration?["ValidIssuer"] ?? throw new ArgumentNullException(nameof(JwtConfig));
            var audience = JwtConfig._configuration?["ValidAudience"] ?? throw new ArgumentNullException(nameof(JwtConfig));
            var expires = 8;
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.secret));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, id), new Claim(ClaimTypes.Role, role) };
            var tokenOptions = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(expires),
                signingCredentials: signinCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
    }
}
