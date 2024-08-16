using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Models.Composite;
using ForumDigitalParadise.Models.Reply;
using ForumDigitalParadise.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Controllers
{
    [Authorize]
    public class ReplyController : Controller
    {
        private readonly IPost _postService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IForum _forumService;
        private readonly IApplicationUser _userService;
        private readonly IPostReply _postReplyService;
        private readonly INotification _notificationService;

        public ReplyController(IPost postService, UserManager<ApplicationUser> userManager, IForum forumService, IApplicationUser userService, IPostReply postReplyService, INotification notificationService)
        {
            _postService = postService;
            _userManager = userManager;
            _forumService = forumService;
            _userService = userService;
            _postReplyService = postReplyService;
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddReply(CompositeViewPostIndexModel model)
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            var reply = BuildReply(model.PostReplyModel, user);
            await _postReplyService.AddReply(reply);
            await _userService.UpdateUserRating(userId, typeof(PostReply));
            await _userService.UpdateUserCommentRating(userId, typeof(PostReply));

            var post = _postService.GetById(model.PostReplyModel.PostId);
            var webrootpathUser = "/img/ProfileImages/";
            string userProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ? "/img/users/default_image.jpg" : webrootpathUser + user.ProfileImageUrl;

            if (post.UserId != userId)
            {
                var author = await _userManager.FindByIdAsync(post.UserId);
                var message = $"<a href='/Profile/Overview/{user.Id}'>{user.UserName}</a> commented on your <a href='/Post/Index/{post.Id}'>post</a>.";
                await _notificationService.CreateNotificationAsync(post.UserId, message, userProfileImageUrl, userId);
            }

            TempData["SuccessMessage"] = "Reply added successfully.";

            return RedirectToAction("Index", "Post", new { id = model.PostIndexModel.Id });
        }

        private PostReply BuildReply(PostReplyModel model, ApplicationUser user)
        {
            var post = _postService.GetById(model.PostId);
            var authorImageUrl = Url.Content("~/img/ProfileImages/" + user.ProfileImageUrl);

            return new PostReply
            {
                Post = post,
                Content = model.ReplyContent,
                Created = DateTime.Now,
                User = user,
                UserId = user.Id,
                AuthorImageUrl = authorImageUrl
            };
        }
    }
}
