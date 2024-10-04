using TechLap.API.Enums;

namespace TechLap.API.Models
{
    public class Admin : BaseModel
    { 
        public string Username { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public AdminRole Role { get; set; }
    }
}
