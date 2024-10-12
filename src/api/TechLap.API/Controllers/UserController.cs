using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TechLap.API.Configurations;
using TechLap.API.DTOs.Requests;
using TechLap.API.Mapper;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;
 
namespace TechLap.API.Controllers
{
    public class UserController : BaseController<UserController>
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserRequest request)
        {
            User user = LazyMapper.Mapper.Map<User>(request);
            user = await _userRepository.AddAsync(user);
            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> UserLogin(UserLoginRequest request)
        {
            var role = "User";
            var issuer = JwtConfig._configuration?["ValidIssuer"] ?? throw new ArgumentNullException(nameof(JwtConfig));
            var audience = JwtConfig._configuration?["ValidAudience"] ?? throw new ArgumentNullException(nameof(JwtConfig));
            var expires = 8;
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.secret));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var userLogin = await _userRepository.UserLogin(request.email, request.password);

            if (userLogin.Email.ToLower().Contains("admin"))
            {
                role = "Admin";
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userLogin.Id.ToString()),
                new Claim(ClaimTypes.Role, role),
            };

            var tokeOptions = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(expires),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK, tokenString);
        }
    }
}
