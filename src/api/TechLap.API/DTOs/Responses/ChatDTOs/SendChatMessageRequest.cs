namespace TechLap.API.DTOs.Responses.ChatDTOs
{
    public class SendChatMessageRequest
    {
        public int ReceiverId { get; set; }
        public string MessageContent { get; set; }
    }
}
