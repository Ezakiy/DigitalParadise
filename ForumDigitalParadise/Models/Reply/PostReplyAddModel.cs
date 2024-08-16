namespace ForumDigitalParadise.Models.Reply
{
    public class PostReplyAddModel
    {
        public string ReplyContent { get; set; }
        public DateTime Created { get; set; }
        public string AuthorId { get; set; }
        public int PostId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorImageUrl { get; set; }
        public bool IsAuthorAdmin { get; set; }
    }
}
