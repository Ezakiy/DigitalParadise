using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Models;
using ForumDigitalParadise.Models.Forum;
using ForumDigitalParadise.Models.Home;
using ForumDigitalParadise.Models.Post;
using ForumDigitalParadise.Models.Reply;
using ForumDigitalParadise.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ForumDigitalParadise.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPost _postService;
        private readonly IForum _forumService;
        private readonly IApplicationUser _userService;
        private readonly IPostReply _postReplyService;

        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IPost postService, UserManager<ApplicationUser> userManager, IPostReply postReplyService, IForum forumservice, IApplicationUser userService)
        {
            _logger = logger;
            _postService = postService;
            _userManager = userManager;
            _postReplyService = postReplyService;
            _forumService = forumservice;
            _userService = userService;
        }
        public IActionResult Index(string sortBy = "Recent")
        {
            var model = BuildHomeIndexModel(sortBy);
            ViewBag.SortBy = sortBy;
            return View(model);
        }

        private HomeIndexModel BuildHomeIndexModel(string sortBy)
        {
            var sortedPosts = _postService.GetPostsSorted(sortBy);

            var currentUser = HttpContext.User.Identity.IsAuthenticated
                ? _userManager.GetUserAsync(HttpContext.User).Result : null;
            var profileImageUrl = currentUser != null ? currentUser.ProfileImageUrl : null;

            var posts = sortedPosts.Select(post =>
            {
                var forum = GetForumListingForPost(post);
                var latestReply = GetLatestReplyForPost(post);
                var webrootpath = "~/img/PostImages/";
                var webrootpathUser = "~/img/ProfileImages/";
                var forumId = forum.Id;
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
                    Forum = forum,
                    LatestReply = latestReply,
                    Created = post.Created,
                    PostImageUrl = postImageUrl,
                    PostContent = post.Content,
                    ForumId = forumId,
                    LikesCount = post.Likes.Count(),
                    IsLikedByUser = isLikedByUser
                };
            });

            return new HomeIndexModel
            {
                LatestPosts = posts,
                SearchQuery = "",
            };
        }

        public bool IsAuthorAdmin(ApplicationUser user)
        {
            return _userManager.GetRolesAsync(user)
                .Result.Contains("Admin");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private PostReplyModel GetLatestReplyForPost(Post post)
        {
            var latestReply = _postReplyService.GetLatestReplyForPost(post.Id);
            if (latestReply == null)
            {
                return null;
            }

            var latestReplyUser = _userManager.FindByIdAsync(latestReply.User.Id).Result;
            var webrootpathUser = "~/img/ProfileImages/";

            return new PostReplyModel
            {
                AuthorId = latestReply.User.Id,
                AuthorName = latestReplyUser != null ? latestReplyUser.UserName : "Unknown",
                AuthorImageUrl = webrootpathUser + latestReplyUser != null ? latestReplyUser.ProfileImageUrl : "DefaultImageUrl",
                Created = latestReply.Created,
                ReplyContent = latestReply.Content
            };
        }
        private ForumListingModel GetForumListingForPost(Post post)
        {
            var forum = post.Forum;
            var webrootpathForum = "~/img/ForumImages/";
            return new ForumListingModel
            {
                Id = forum.Id,
                Name = forum.Title,
                ForumImageUrl = webrootpathForum + forum.ImageUrl
            };
        }
    }
}