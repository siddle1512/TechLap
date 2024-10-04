using TechLap.API.Enums;

namespace TechLap.API.Models
{
    public class User : BaseModel
    {
        public string FullName { get; set; } = string.Empty;
        public string BirthYear { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public string Email { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public string AvatarPath { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
