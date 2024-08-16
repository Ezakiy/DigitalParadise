using ForumDigitalParadise.Models.Post;
using ForumDigitalParadise.Models.Home;
using ForumDigitalParadise.Models.Forum;
using ForumDigitalParadise.Models.ApplicationUser;
namespace ForumDigitalParadise.Models.Search
{
    public class SearchResultModel
    {
        public IEnumerable<PostListingModel> Posts { get; set; }
        public IEnumerable<ForumListingModel> Forums { get; set; }
        public IEnumerable<ProfileListModel> Users { get; set; }
        public string SearchQuery { get; set; }
        public bool EmptySearchResults { get; set; }
    }
}
