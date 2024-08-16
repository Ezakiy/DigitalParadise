using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Models.ApplicationUser;
using ForumDigitalParadise.Models.Forum;
using ForumDigitalParadise.Models.Post;
using ForumDigitalParadise.Models.Search;
using ForumDigitalParadise.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPost _postService;
        private readonly IForum _forumService;
        private readonly IApplicationUser _userService;
        private readonly IForumFollower _forumFollowerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SearchController(IPost postService, IForum forumService, IApplicationUser userService, IForumFollower forumFollowerService, UserManager<ApplicationUser> userManager)
        {
            _postService = postService;
            _forumService = forumService;
            _userService = userService;
            _forumFollowerService = forumFollowerService;
            _userManager = userManager;
        }

        [HttpPost]
        public IActionResult Search(string searchQuery, string sortBy = "Recent")
        {
            return RedirectToAction("Results", new { searchQuery, sortBy });
        }

        public async Task<IActionResult> Results(string searchQuery, string sortBy = "Recent")
        {
            var posts = _postService.SearchPosts(searchQuery);
            var forums = _forumService.SearchForums(searchQuery);
            var areNoResults = (!string.IsNullOrEmpty(searchQuery) && !posts.Any());
            var currentUser = HttpContext.User.Identity.IsAuthenticated
                ? await _userManager.GetUserAsync(HttpContext.User) : null;

            var sortedPosts = _postService.GetPostsSorted(sortBy);

            var postListings = sortedPosts.Select(post =>
            {
                var webrootpath = "~/img/PostImages/";
                var webrootpathUser = "~/img/ProfileImages/";
                var postImageUrl = string.IsNullOrEmpty(post.ImageUrl) ? null : webrootpath + post.ImageUrl;

                bool isLikedByUser = currentUser != null && post.Likes.Any(like => like.UserId == currentUser.Id);

                return new PostListingModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    AuthorName = post.User.DisplayName,
                    AuthorImageUrl = string.IsNullOrEmpty(post.User.ProfileImageUrl) ? "~/img/users/default_image.jpg" : webrootpathUser + post.User.ProfileImageUrl,
                    AuthorId = post.User.Id,
                    AuthorRating = post.User.Rating,
                    DatePosted = post.Created.ToString(),
                    RepliesCount = post.Replies.Count(),
                    IsAuthorAdmin = IsAuthorAdmin(post.User),
                    Views = post.Views,
                    Created = post.Created,
                    PostImageUrl = postImageUrl,
                    PostContent = post.Content,
                    LikesCount = post.Likes.Count(),
                    IsLikedByUser = isLikedByUser
                };
            });

            var forumListings = new List<ForumListingModel>();
            foreach (var forum in forums)
            {
                var memberCount = await _forumFollowerService.GetForumFollowersCountAsync(forum.Id);
                forumListings.Add(new ForumListingModel
                {
                    Id = forum.Id,
                    Name = forum.Title,
                    Description = forum.Description,
                    ForumImageUrl = string.IsNullOrEmpty(forum.ImageUrl) ? "" : Url.Content("~/img/ForumImages/" + forum.ImageUrl),
                    MembersCount = memberCount
                });
            }

            var model = new SearchResultModel
            {
                Posts = postListings,
                Forums = forumListings,
                EmptySearchResults = areNoResults,
                SearchQuery = searchQuery
            };

            ViewBag.SortBy = sortBy;
            ViewBag.SearchQuery = searchQuery;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetForumFollowersCount(int forumId)
        {
            var followerCount = await _forumFollowerService.GetForumFollowersCountAsync(forumId);
            return Json(new { followerCount });
        }

        public bool IsAuthorAdmin(ApplicationUser user)
        {
            return _userManager.GetRolesAsync(user)
                .Result.Contains("Admin");
        }

        [HttpGet]
        public JsonResult GetSuggestions(string query)
        {
            var posts = _postService.SearchPosts(query).Select(p => new { name = p.Title }).ToList();
            var forums = _forumService.SearchForums(query).Select(f => new { name = f.Title }).ToList();
            var suggestions = posts.Concat(forums).ToList();
            return Json(suggestions);
        }
    }

}
