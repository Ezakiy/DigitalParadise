using Microsoft.AspNetCore.Identity;
using System;

namespace ForumDigitalParadise.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int Rating { get; set; }
        public int PostRating { get; set; }
        public int CommentRating { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? BannerImageUrl { get; set; }
        public DateTime MemberSince { get; set; }
        public bool IsActive { get; set; }
        public string? UserDescription { get; set; }
        public string? DisplayName { get; set; }

        public int DisplayId { get; set; }

        public virtual ICollection<Follower> Followers { get; set; } = new List<Follower>();
        public virtual ICollection<Follower> Followees { get; set; } = new List<Follower>();

        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

        public ApplicationUser()
        {
            MemberSince = DateTime.UtcNow;
        }
    }
}
