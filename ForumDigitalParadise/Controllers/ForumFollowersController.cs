using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ForumFollowersController : ControllerBase
    {
        private readonly IForumFollower _forumFollowerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForumFollowersController(IForumFollower forumFollower, UserManager<ApplicationUser> userManager)
        {
            _forumFollowerService = forumFollower;
            _userManager = userManager;
        }

        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromBody] int forumId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _forumFollowerService.FollowForumAsync(user.Id, forumId);
            return Ok(new { success = true });
        }

        [HttpPost("unfollow")]
        public async Task<IActionResult> Unfollow([FromBody] int forumId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _forumFollowerService.UnfollowForumAsync(user.Id, forumId);
            return Ok(new { success = true });
        }

        [HttpGet("isFollowing/{forumId}")]
        public async Task<IActionResult> IsFollowing(int forumId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isFollowing = await _forumFollowerService.IsFollowingForumAsync(user.Id, forumId);
            return Ok(new { isFollowing });
        }

        [HttpGet("followedForums")]
        public async Task<IActionResult> GetFollowedForums()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followedForums = await _forumFollowerService.GetFollowedForumsAsync(userId);

            var webrootpathForum = "~/img/ForumImages/";

            var followedForumsList = followedForums.Select(forum => new
            {
                forum.Id,
                forum.Title,
                forum.Description,
                forum.ImageUrl
            }).ToList();

            return Ok(followedForumsList);
        }

    }
}
