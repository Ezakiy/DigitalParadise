namespace ForumDigitalParadise.Models.Forum
{
    public class EditForumModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MiniDescription { get; set; }
        public string? ForumImageUrl { get; set; }
    }
}
