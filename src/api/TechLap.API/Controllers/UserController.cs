using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.UserDTOs;
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("/api/users/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var users = await _userRepository.GetByIdAsync(id);
            var response = LazyMapper.Mapper.Map<UserResponse>(users);
            return CreateResponse<UserResponse>(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("/api/users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllAsync(o => true);
            var response = LazyMapper.Mapper.Map<IEnumerable<UserResponse>>(users);
            return CreateResponse<IEnumerable<UserResponse>>(true, "Request processed successfully.", HttpStatusCode.OK, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("/api/users")]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            User user = LazyMapper.Mapper.Map<User>(request);
            user = await _userRepository.AddAsync(user);
            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK, "Add userId " + user.Id + " successfully");
        }

        [HttpPost]
        [Route("/api/user/login")]
        public async Task<IActionResult> UserLogin(UserLoginRequest request)
        {
            var userLogin = await _userRepository.UserLogin(request.email, request.password);
            var tokenString = GenerateJwtToken(userLogin.Id.ToString(), "User");
            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK, tokenString);
        }

        [HttpGet]
        [Authorize]
        [Route("/api/user/validateToken")]
        public IActionResult ValidateToken()
        {
            return CreateResponse<string>(true, "Request processed successfully.", HttpStatusCode.OK);
        }
    }
}
