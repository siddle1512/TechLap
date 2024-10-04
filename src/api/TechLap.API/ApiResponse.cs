namespace TechLap.API
{
    public class ApiResponse<Response>
    {
        public bool IsSuccess { get; set; }
        public Response? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
