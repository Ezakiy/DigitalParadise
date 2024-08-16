using System;

namespace ForumDigitalParadise.Data.Models
{
    public class Like
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
        public DateTime Created { get; set; }

        public Like()
        {
            Created = DateTime.UtcNow;
        }
    }
}
