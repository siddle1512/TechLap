using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        public string BirthYear { get; set; }
        public Gender Gender { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string HashedPassword { get; set; }
        public string AvatarPath { get; set; }
        public string AddressPath { get; set; }
        public UserStatus Status { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
