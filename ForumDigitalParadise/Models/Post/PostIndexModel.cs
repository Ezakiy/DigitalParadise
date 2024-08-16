using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Models.Reply;

namespace ForumDigitalParadise.Models.Post
{
    public class PostIndexModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorUserName { get; set; }
        public string AuthorImageUrl { get; set; }
        public int AuthorRating { get; set; }
        public DateTime Created { get; set; }
        public string PostContent { get; set; }
        public bool IsAuthorAdmin { get; set; }
        public int ForumId { get; set; }
        public string ForumName { get; set; }
        public string ForumDescription { get; set; }
        public string ForumMiniDescription { get; set; }
        public string ForumImageUrl { get; set; }
        public string? PostImageUrl { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public IEnumerable<PostReplyModel> Replies { get; set; }
        public string RepliesCount { get; set; }
        public bool IsLikedByUser { get; set; }
    }
}
