using ForumDigitalParadise.Models.Forum;
using ForumDigitalParadise.Models.Post;
using ForumDigitalParadise.Models.Reply;

namespace ForumDigitalParadise.Models.Composite
{
    public class CompositeViewPostIndexModel
    {
        public PostIndexModel PostIndexModel { get; set; }
        public PostReplyModel PostReplyModel { get; set; }
        public ForumTopicModel ForumTopicModel { get; set; }
        public PostListingModel PostListingModel { get; set; }
    }
}
