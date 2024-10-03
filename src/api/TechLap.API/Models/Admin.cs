using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string HashedPassword { get; set; }
        public AdminRole Role { get; set; }
    }
}
