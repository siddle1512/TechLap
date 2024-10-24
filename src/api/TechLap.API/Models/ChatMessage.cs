using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        public int SenderId { get; set; } 
        public int ReceiverId { get; set; } 
        public string MessageContent { get; set; } = string.Empty;
        public bool IsFromAdmin { get; set; }  
        public DateTime SentAt { get; set; }
    }
}
