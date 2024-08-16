

using ForumDigitalParadise.Models.Post;

namespace ForumDigitalParadise.Models.Forum
{
    public class ForumTopicModel    
    {
        public ForumListingModel Forum {  get; set; }
        public IEnumerable<PostListingModel>? Posts { get; set; }
        public string SearchQuery { get; set; }
        //public string? PostImageUrl { get; set; }
    }
}
