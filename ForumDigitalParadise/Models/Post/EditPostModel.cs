namespace ForumDigitalParadise.Models.Post
{
    public class EditPostModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? PostImageUrl { get; set; }
    }
}
