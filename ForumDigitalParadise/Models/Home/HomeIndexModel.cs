using ForumDigitalParadise.Models.Post;

namespace ForumDigitalParadise.Models.Home
{
    public class HomeIndexModel
    {
        public string SearchQuery { get; set; }
        public int ForumId { get; set; }
        public IEnumerable<PostListingModel> LatestPosts { get; set; }
        
    }
}
