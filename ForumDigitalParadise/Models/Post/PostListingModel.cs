using ForumDigitalParadise.Models.Forum;
using ForumDigitalParadise.Models.Reply;

namespace ForumDigitalParadise.Models.Post
{
    public class PostListingModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string AuthorUserName { get; set; }
        public int AuthorRating { get; set; }
        public string AuthorId { get; set; }
        public string DatePosted { get; set; }
        public string AuthorImageUrl { get; set; }

        public bool IsAuthorAdmin { get; set; }
        public bool IsOwner { get; set; }
        public string ForumName { get; set; }
        public ForumListingModel Forum { get; set; }
        public int ForumId { get; set; }
        public PostReplyModel LatestReply { get; set; }

        public int RepliesCount { get; set; }
        public int Views { get; set; }
        public DateTime Created { get; set; }

        public string? PostImageUrl { get; set; }
        public string PostContent { get; set; }

        public int LikesCount { get; set; }
        public bool IsLikedByUser { get; set; }
    }
}
