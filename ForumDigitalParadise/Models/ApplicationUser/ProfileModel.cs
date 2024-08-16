using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Models.Post;

namespace ForumDigitalParadise.Models.ApplicationUser
{
    public class ProfileModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string UserRating { get; set; }
        public string UserPostRating { get; set; }
        public string UserCommentRating { get; set; }
        public string UserDescription {  get; set; }
        public string ProfileImageUrl { get; set; }
        public string? BannerImageUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsOwner { get; set; }
        public DateTime MemberSince { get; set; }

        public IEnumerable<PostListingModel> Posts { get; set; }
        public IEnumerable<PostReply> Replies { get; set; }
        public IEnumerable<Like> Likes { get; set; }
    }
}
