using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebFeedback.Domain.Entities
{
    public class Dislike : Abstractions.BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }


        [Required]
        public int ReviewId { get; set; }

        [JsonIgnore]
        public required Review Review { get; set; }
    }
}
