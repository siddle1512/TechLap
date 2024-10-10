using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TechLap.API.Configurations;
using TechLap.API.DTOs.Requests;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Controllers
{
    public class UserController : BaseController<UserController>
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> UserLogin(UserLoginRequest request)
        {
            var role = "User";
            var issuer = JwtConfig._configuration?["ValidIssuer"] ?? throw new ArgumentNullException(nameof(JwtConfig));
            var audience = JwtConfig._configuration?["ValidAudience"] ?? throw new ArgumentNullException(nameof(JwtConfig));
            var expires = 60;
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.secret));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var userLogin = await _userRepository.UserLogin(request.email, request.password);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userLogin.Id.ToString()),
                new Claim(ClaimTypes.Role, role),
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expires),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK, tokenString);
        }
    }
}
