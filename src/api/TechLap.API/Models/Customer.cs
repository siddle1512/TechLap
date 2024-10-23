using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class Customer : BaseModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public IEnumerable<Order> Orders { get; set; } = new List<Order>();
    }
}
