using System.ComponentModel.DataAnnotations;

namespace ForumDigitalParadise.Data.Models
{
    public class PostReply
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime Created { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }

        public string? AuthorImageUrl { get; set; }
    }
}

