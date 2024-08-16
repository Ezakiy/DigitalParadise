using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ForumDigitalParadise.Models.Followers;

namespace ForumDigitalParadise.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MyFollowersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFollower _followerService;
        private readonly IApplicationUser _applicationUserService;

        public MyFollowersController(UserManager<ApplicationUser> userManager, IFollower followerService, IApplicationUser applicationUserService)
        {
            _userManager = userManager;
            _followerService = followerService;
            _applicationUserService = applicationUserService;
        }

        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromBody] FollowRequest model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                await _followerService.FollowUserAsync(currentUser.Id, model.FollowerId);
                return Ok(new { success = true });
            }
            catch
            {
                return BadRequest(new { success = false, message = "Failed to follow the user." });
            }
        }

        [HttpPost("unfollow")]
        public async Task<IActionResult> Unfollow([FromBody] FollowRequest model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            try
            {
                await _followerService.UnfollowUserAsync(currentUser.Id, model.FollowerId);
                return Ok(new { success = true });
            }
            catch
            {
                return BadRequest(new { success = false, message = "Failed to unfollow the user." });
            }
        }

        [HttpGet("isFollowing/{id}")]
        public async Task<IActionResult> IsFollowing(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var isFollowing = await _followerService.IsFollowingAsync(currentUser.Id, id);
            return Ok(new { isFollowing });
        }

        [HttpGet("isFollower/{id}")]
        public async Task<IActionResult> IsFollower(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var isFollower = await _followerService.IsFollowerAsync(id, currentUser.Id);
            return Ok(new { isFollower });
        }

        [HttpGet("followingCount/{userId}")]
        public async Task<IActionResult> GetFollowingCount(string userId)
        {
            var count = await _followerService.GetFollowingCountAsync(userId);
            return Ok(new { count });
        }
    }
}
