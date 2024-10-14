using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechLap.API.Enums;

namespace TechLap.API.Models
{
    public class User : BaseModel
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public DateTime BirthYear { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(10, MinimumLength = 10)]
        [RegularExpression(@"^0[1-9]\d{8}$")]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string HashedPassword { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(max)")]
        public string AvatarPath { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(max)")]
        public string Address { get; set; } = string.Empty;
        [Required]
        public UserStatus Status { get; set; }
        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
