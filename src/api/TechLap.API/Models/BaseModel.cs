using System.ComponentModel.DataAnnotations;

namespace TechLap.API.Models
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
