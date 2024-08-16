using System;

namespace ForumDigitalParadise.Data.Models
{
    public class ForumFollower
    {
        public int ForumFollowerId { get; set; }
        public int ForumId { get; set; }
        public string UserId { get; set; }

        public DateTime FollowingDate { get; set; }

        public virtual Forum Forum { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
