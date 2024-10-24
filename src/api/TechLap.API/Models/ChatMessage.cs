using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; } 
    }
}
