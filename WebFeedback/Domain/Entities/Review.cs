using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace WebFeedback.Domain.Entities
{
    public class Review : Abstractions.BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public required string Text { get; set; }

        [Required]
        public int Rate { get; set; }

        [JsonIgnore]
        public ICollection<Comment>? Comments { get; set; }

        [JsonIgnore]
        public ICollection<Like>? Likes { get; set; }


        [JsonIgnore]
        public ICollection<Dislike>? Dislikes { get; set; }
    }
}
