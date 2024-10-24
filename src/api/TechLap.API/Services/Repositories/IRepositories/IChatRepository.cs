using TechLap.API.Models;

namespace TechLap.API.Services.Repositories.IRepositories
{
    public interface IChatRepository : IAsyncRepository<ChatMessage>
    {
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(int userId, int adminId);
        Task<ChatMessage> SendChatMessageAsync(ChatMessage message);
    }
}
