namespace TechLap.API.DTOs.Responses.ChatDTOs
{
    public class ChatMessageResponse
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsFromAdmin { get; set; }
    }
}
