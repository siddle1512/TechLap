using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime? CreatedDate { get; set; }
        [Required]
        public DateTime? LastModifiedDate { get; set; }
    }
}
