using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TechLap.API.Data;
using TechLap.API.Enums;
using TechLap.API.Exceptions;
using TechLap.API.Models;
using TechLap.API.Services.Repositories.IRepositories;

namespace TechLap.API.Services.Repositories.Repositories
{
    public class ChatRepository : RepositoryBase, IChatRepository
    {
        public ChatRepository(TechLapContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(int userId, int adminId)
        {
            var chatMessages = await _dbContext.ChatMessages
                .Where(m => (m.SenderId == userId && m.ReceiverId == adminId) ||
                            (m.SenderId == adminId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
            return chatMessages;
        }

        public async Task<ChatMessage> SendChatMessageAsync(ChatMessage message)
        {
            // Kiểm tra người gửi tồn tại và active
            var sender = await _dbContext.Users.FindAsync(message.SenderId);
            if (sender == null)
            {
                throw new NotFoundException($"Sender with ID {message.SenderId} not found");
            }
            if (sender.Status != UserStatus.Active)
            {
                throw new InvalidOperationException($"Sender account is not active");
            }

            // Kiểm tra người nhận tồn tại và active  
            var receiver = await _dbContext.Users.FindAsync(message.ReceiverId);
            if (receiver == null)
            {
                throw new NotFoundException($"Receiver with ID {message.ReceiverId} not found");
            }
            if (receiver.Status != UserStatus.Active)
            {
                throw new InvalidOperationException($"Receiver account is not active");
            }

            // Kiểm tra nếu người gửi là User thì người nhận phải là Admin
            if (!message.IsFromAdmin)
            {
                var isReceiverAdmin = await _dbContext.Admins.AnyAsync(a => a.Id == message.ReceiverId);
                if (!isReceiverAdmin)
                {
                    throw new InvalidOperationException("Users can only send messages to Admins");
                }
            }

            await _dbContext.ChatMessages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
            return message;
        }


        public Task<bool> DeleteAsync(ChatMessage entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ChatMessage>> GetAllAsync(Expression<Func<ChatMessage, bool>> predicate)
        {
            var chatMessages = await _dbContext.ChatMessages.Where(predicate).ToListAsync();
            if (!chatMessages.Any())
            {
                throw new NotFoundException("No chat messages found");
            }
            return chatMessages;
        }

        public async Task<ChatMessage?> GetByIdAsync(int id)
        {
            var message = await _dbContext.ChatMessages.FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                throw new NotFoundException($"No chat message found with id: {id}");
            }
            return message;
        }

        public async Task<bool> UpdateAsync(ChatMessage entity)
        {
            _dbContext.ChatMessages.Entry(entity).State = EntityState.Modified;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public Task<ChatMessage> AddAsync(ChatMessage entity)
        {
            throw new NotImplementedException();
        }
    }
}
