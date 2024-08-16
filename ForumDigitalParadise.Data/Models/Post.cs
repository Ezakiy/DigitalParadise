using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; }
        public int Views { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Forum Forum { get; set; }
        public virtual ICollection<PostReply> Replies { get; set; } = new List<PostReply>();
        public string? ImageUrl { get; set; }

        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

        public int GetRepliesCount()
        {
            return Replies.Count;
        }

        public int GetLikesCount()
        {
            return Likes.Count;
        }
    }
}
