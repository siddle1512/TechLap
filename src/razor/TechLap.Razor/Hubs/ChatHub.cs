using Microsoft.AspNetCore.SignalR;
using TechLap.API.Data;
using TechLap.API.Models;

namespace TechLap.Razor.Hubs
{
    public class ChatHub : Hub
    {
        private readonly TechLapContext _context;

        public ChatHub(TechLapContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string senderId, string receiverId, string message, bool isFromAdmin)
        {
            // Broadcast the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", senderId, message);

            // Save the message in the ChatMessages table
            var chatMessage = new ChatMessage
            {
                SenderId = int.Parse(senderId),
                ReceiverId = int.Parse(receiverId),
                MessageContent = message,
                IsFromAdmin = isFromAdmin,
                SentAt = DateTime.Now
            };

            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();  // Save changes to the database
        }
    }
}
