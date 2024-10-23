using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using TechLap.API.Data;
using TechLap.API.DTOs.Responses.ChatDTOs;
using TechLap.API.Mapper;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : BaseController<ChatController>
    {
        private readonly IChatRepository _chatRepository;

        public ChatController(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
        }

        // Lấy lịch sử chat
        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        [Route("/api/chat/history")]
        public async Task<IActionResult> GetChatHistory(int userId, int adminId)
        {
            var chatMessages = await _chatRepository.GetChatHistoryAsync(userId, adminId);
            var response = LazyMapper.Mapper.Map<IEnumerable<ChatMessageResponse>>(chatMessages);
            return CreateResponse<IEnumerable<ChatMessageResponse>>(true, "Chat history retrieved successfully.", HttpStatusCode.OK, response);
        }

        // Gửi tin nhắn
        [Authorize(Roles = "Admin, User")]
        [HttpPost]
        [Route("/api/chat/send")]
        public async Task<IActionResult> SendChatMessage(SendChatMessageRequest request)
        {
            var senderId = GetUserIdFromToken();
            if (senderId == null)
                return CreateResponse<string>(false, "Sender not found", HttpStatusCode.Unauthorized);

            var chatMessage = LazyMapper.Mapper.Map<ChatMessage>(request);
            chatMessage.SenderId = senderId.Value;
            chatMessage.SentAt = DateTime.UtcNow;

            // Thiết lập IsFromAdmin dựa trên vai trò của người gửi
            var userRole = GetUserRoleFromToken(); // Lấy vai trò của người gửi
            chatMessage.IsFromAdmin = userRole == "Admin"; // Nếu là admin, IsFromAdmin sẽ là true (1), ngược lại là false (0)

            chatMessage = await _chatRepository.SendChatMessageAsync(chatMessage);
            return CreateResponse<string>(true, "Message sent successfully.", HttpStatusCode.OK, "Message sent");
        }

        protected string GetUserRoleFromToken()
        {
            var roleClaim = User.FindFirstValue(ClaimTypes.Role); // Lấy thông tin vai trò từ claims
            if (string.IsNullOrEmpty(roleClaim))
            {
                throw new AuthenticationException("Unauthorized: Role not found.");
            }
            return roleClaim; // Trả về vai trò của người dùng
        }


    }
}
