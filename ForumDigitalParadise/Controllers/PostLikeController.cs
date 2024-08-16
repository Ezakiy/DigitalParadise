using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class PostLikeController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPostLike _postLikeService;
    private readonly IPost _postService;
    private readonly INotification _notificationService;

    public PostLikeController(IPostLike postLikeService, IPost postService, UserManager<ApplicationUser> userManager, INotification notificationService)
    {
        _postLikeService = postLikeService;
        _postService = postService;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    [HttpPost("{postId}/like")]
    public async Task<IActionResult> LikePost(int postId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized("User is not authenticated.");

        try
        {
            await _postLikeService.LikePostAsync(postId, userId);

            // Retrieve the post and the author
            var post = await _postService.GetByIdAsync(postId);
            if (post == null)
                return NotFound("Post not found.");

            var author = await _userManager.FindByIdAsync(post.UserId);
            if (author == null)
                return NotFound("Post author not found.");

            // Create the notification message
            var liker = await _userManager.FindByIdAsync(userId);
            var likerProfileImageUrl = string.IsNullOrEmpty(liker.ProfileImageUrl) ? "/img/users/default_image.jpg" : $"/img/ProfileImages/{liker.ProfileImageUrl}";
            var notificationMessage = $"<a href='/Profile/Overview/{userId}'>{liker.DisplayName}</a> liked your post: <a href='/Post/Index/{postId}'>{post.Title}</a>";

            // Send notification to the post's author
            await _notificationService.CreateNotificationAsync(post.UserId, notificationMessage, likerProfileImageUrl, userId);

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }


    [HttpPost("{postId}/unlike")]
    public async Task<IActionResult> UnlikePost(int postId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized("User is not authenticated.");

        try
        {
            await _postLikeService.UnlikePostAsync(postId, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{postId}/likes")]
    public async Task<IActionResult> GetLikesCount(int postId)
    {
        try
        {
            var count = await _postLikeService.GetLikesCountAsync(postId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{postId}/islikedbyuser")]
    public async Task<IActionResult> IsPostLikedByUser(int postId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized("User is not authenticated.");

        try
        {
            var isLiked = await _postLikeService.IsPostLikedByUserAsync(postId, userId);
            return Ok(isLiked);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

}