using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ForumDigitalParadise.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FollowersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFollower _followerService;
        private readonly IApplicationUser _applicationUserService;

        public FollowersController(UserManager<ApplicationUser> userManager, IFollower followerService, IApplicationUser applicationUserService)
        {
            _userManager = userManager;
            _followerService = followerService;
            _applicationUserService = applicationUserService;
        }

        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromBody] string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userToFollow = _applicationUserService.GetById(id);
            if (userToFollow == null)
            {
                return NotFound();
            }

            await _followerService.FollowUserAsync(currentUser.Id, id);
            return Ok(new { success = true });
        }

        [HttpPost("unfollow")]
        public async Task<IActionResult> Unfollow([FromBody] string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userToUnfollow = _applicationUserService.GetById(id);
            if (userToUnfollow == null)
            {
                return NotFound();
            }

            await _followerService.UnfollowUserAsync(currentUser.Id, id);
            return Ok(new { success = true });
        }


        [HttpPost("followback")]
        public async Task<IActionResult> FollowBack([FromBody] string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (currentUser.Id == id)
            {
                return BadRequest(new { message = "You cannot follow yourself." });
            }

            var userToFollow = _applicationUserService.GetById(id);
            if (userToFollow == null)
            {
                return NotFound();
            }

            await _followerService.FollowUserAsync(currentUser.Id, id);
            return Ok(new { success = true });
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

        [HttpGet("followedUsers")]
        public async Task<IActionResult> GetFollowecdUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var followedUsers = await _followerService.GetFollowedUsersAsync(userId);
            var webrootpathUser = "~/img/ProfileImages/";

            var followedUsersList = followedUsers.Select(user => new
            {
                user.Id,
                user.DisplayName,
                user.ProfileImageUrl
            }).ToList();

            Console.WriteLine("Followed Users: ");
            foreach (var user in followedUsersList)
            {
                Console.WriteLine($"Id: {user.Id}, DisplayName: {user.DisplayName}, ProfileImageUrl: {user.ProfileImageUrl}");
            }

            return Ok(followedUsersList);
        }

    }
}
