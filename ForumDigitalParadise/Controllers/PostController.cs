using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Data;
using ForumDigitalParadise.Models.Composite;
using ForumDigitalParadise.Models.Post;
using ForumDigitalParadise.Models.Reply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ForumDigitalParadise.Models.Forum;
using ForumDigitalParadise.Service;

public class PostController : Controller
{
    private readonly IPost _postService;
    private readonly IForum _forumService;
    private static UserManager<ApplicationUser> _userManager;
    private readonly IApplicationUser _userService;
    private readonly IPostReply _postReplyService;
    private readonly IUpload _uploadService;
    private readonly IForumFollower _forumFollowerService;
    private readonly IFollower _followerService;
    private readonly INotification _notificationService;
    public PostController(IPost postService, IForum forumService, UserManager<ApplicationUser> userManager, IApplicationUser userService, IPostReply postReply, IUpload upload, IForumFollower forumFollowerService, INotification notificationService, IFollower followerService)
    {
        _postService = postService;
        _forumService = forumService;
        _userManager = userManager;
        _userService = userService;
        _postReplyService = postReply;
        _uploadService = upload;
        _forumFollowerService = forumFollowerService;
        _notificationService = notificationService;
        _followerService = followerService;
    }

    public async Task<IActionResult> Index(int id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId != null)
        {
            var hasViewedPost = await _postService.HasUserViewedPost(userId, id);
            if (!hasViewedPost)
            {
                await _postService.RecordPostView(userId, id);
            }
        }

        try
        {
            var post = _postService.GetById(id);
            if (post == null)
            {
                return RedirectToAction("Detail", "ErrorPage");
            }

            var forum = _forumService.GetById(post.Forum.Id);
            if (forum == null)
            {
                return RedirectToAction("Detail", "ErrorPage");
            }

            var currentUser = HttpContext.User.Identity.IsAuthenticated ? await _userManager.GetUserAsync(HttpContext.User) : null;
            var profileImageUrl = currentUser?.ProfileImageUrl;
            var webrootpathUser = "~/img/ProfileImages/";
            var webrootpathPost = "~/img/PostImages/";
            var webrootpathForum = "~/img/ForumImages/";

            bool isLikedByUser = currentUser != null && post.Likes.Any(like => like.UserId == currentUser.Id);

            var model = new CompositeViewPostIndexModel
            {
                PostIndexModel = new PostIndexModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    AuthorId = post.User.Id,
                    AuthorName = post.User.DisplayName,
                    AuthorUserName = post.User.UserName,
                    AuthorImageUrl = string.IsNullOrEmpty(post.User.ProfileImageUrl) ? "~/img/users/default_image.jpg" : webrootpathUser + post.User.ProfileImageUrl,
                    AuthorRating = post.User.Rating,
                    Created = post.Created,
                    PostContent = post.Content,
                    Replies = BuildPostReplies(post.Replies),
                    ForumId = post.Forum.Id,
                    ForumName = post.Forum.Title,
                    ForumDescription = post.Forum.Description,
                    ForumMiniDescription = post.Forum.MiniDescription,
                    ForumImageUrl = string.IsNullOrEmpty(post.Forum.ImageUrl) ? "~/img/users/default_image.jpg" : webrootpathForum + post.Forum.ImageUrl,
                    IsAuthorAdmin = IsAuthorAdmin(post.User),
                    PostImageUrl = string.IsNullOrEmpty(post.ImageUrl) ? null : webrootpathPost + post.ImageUrl,
                    Likes = post.Likes,
                    IsLikedByUser = isLikedByUser
                },
                PostReplyModel = new PostReplyModel { PostId = post.Id },
                ForumTopicModel = new ForumTopicModel
                {
                    Forum = new ForumListingModel
                    {
                        Id = forum.Id,
                        Name = forum.Title,
                        Description = forum.Description,
                        MiniDescription = forum.MiniDescription,
                        ForumImageUrl = string.IsNullOrEmpty(forum.ImageUrl) ? "~/img/users/default_image.jpg" : webrootpathForum + forum.ImageUrl,
                        NumberOfPosts = forum.Posts.Count(),
                        HasRecentPosts = forum.Posts.Any(p => p.Created > DateTime.Now.AddDays(-7))
                    },
                    Posts = post.Forum.Posts.Select(p => new PostListingModel
                    {
                        Id = p.Id,
                        Title = p.Title,
                        AuthorId = p.User.Id,
                        AuthorName = p.User.DisplayName,
                        AuthorUserName = p.User.UserName,
                        AuthorImageUrl = string.IsNullOrEmpty(p.User.ProfileImageUrl) ? "~/img/users/default_image.jpg" : webrootpathUser + p.User.ProfileImageUrl,
                        Created = p.Created,
                        RepliesCount = p.Replies.Count,
                        ForumId = p.Forum.Id,
                        ForumName = p.Forum.Title,
                        LikesCount = p.Likes.Count,
                        IsLikedByUser = isLikedByUser
                    })
                },
                PostListingModel = new PostListingModel
                {
                    RepliesCount = post.Replies.Count,
                    LikesCount = post.Likes.Count
                }
            };

            return View(model);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }



    [Authorize]
    public IActionResult Create(int id)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Detail", "ErrorPage");
        }

        Console.WriteLine($"Received Forum ID: {id}");

        var forum = _forumService.GetById(id);
        if (forum == null)
        {
            Console.WriteLine($"Forum with ID {id} not found.");

            return NotFound("Forum not found.");
        }

        var model = new NewPostModel
        {
            ForumName = forum.Title,
            ForumId = forum.Id,
            ForumImageUrl = forum.ImageUrl,
            AuthorName = User.Identity.Name
        };

        return View(model);
    }


    [Authorize]
    public IActionResult Edit(int id)
    {
        var post = _postService.GetById(id);

        if (post == null)
        {
            return RedirectToAction("Detail", "ErrorPage");
        }

        var webrootpathPost = "~/img/PostImages/";
        var imageUrl = string.IsNullOrEmpty(post.ImageUrl) ? null : webrootpathPost + post.ImageUrl;

        var model = new EditPostModel
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            PostImageUrl = imageUrl
        };

        return View(model);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(EditPostModel model, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var post = _postService.GetById(model.Id);

        if (post == null)
        {
            return NotFound();
        }

        post.Title = model.Title;
        post.Content = model.Content;

        if (file != null && file.Length > 0)
        {
            var postImageUrl = await _uploadService.UploadPostImage(file);
            post.ImageUrl = postImageUrl;
            model.PostImageUrl = postImageUrl;  
        }

        await _postService.UpdatePost(post);

        TempData["SuccessMessage"] = "Post edited successfully.";
        return RedirectToAction("Index", "Post", new { id = post.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _postService.DeletePostAndRelatedRecords(id);

            TempData["SuccessMessage"] = "Post deleted successfully.";

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ViewPost(int id)
    {
        var userId = _userManager.GetUserId(User);

        if (userId != null)
        {
            if (!await _postService.HasUserViewedPost(userId, id))
            {
                await _postService.RecordPostView(userId, id);
            }
        }

        var post = _postService.GetById(id);

        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }

    private bool IsAuthorAdmin(ApplicationUser user)
    {
        return _userManager.GetRolesAsync(user).Result.Contains("Admin");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddPost(NewPostModel model, IFormFile? file)
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.FindByIdAsync(userId);
        var post = BuildPost(model, user);

        if (file != null && file.Length > 0)
        {
            var postImageUrl = await _uploadService.UploadPostImage(file);
            post.ImageUrl = postImageUrl;
            model.PostImageUrl = postImageUrl;
        }

        await _postService.Add(post);
        await _userService.UpdateUserRating(userId, typeof(Post));
        await _userService.UpdateUserPostRating(userId, typeof(Post));

        var forum = _forumService.GetById(model.ForumId);
        var forumName = forum != null ? forum.Title : "the forum";

        var forumFollowers = await _forumFollowerService.GetForumFollowersAsync(model.ForumId);
        var notificationMessage = $"New post in <a href='/Forum/Topic/{forum.Id}'>{forumName}</a>: {post.Title}";
        var webrootpathUser = "/img/ProfileImages/";

        string userProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ? "/img/users/default_image.jpg" : webrootpathUser + user.ProfileImageUrl;

        foreach (var follower in forumFollowers)
        {
            await _notificationService.CreateNotificationAsync(follower.UserId, notificationMessage, userProfileImageUrl, userId);
        }

        var userFollowers = await _followerService.GetFollowersAsync(userId);
        var userPostNotificationMessage = $"<a href='/Profile/Overview/{userId}'>{user.DisplayName}</a> posted: <a href='/Post/Index/{post.Id}'>{post.Title}</a>";

        foreach (var follower in userFollowers)
        {
            await _notificationService.CreateNotificationAsync(follower.FollowerId, userPostNotificationMessage, userProfileImageUrl, userId);
        }

        TempData["SuccessMessage"] = "Post created successfully!";
        return RedirectToAction("Index", "Post", new { id = post.Id });
    }


    private Post BuildPost(NewPostModel model, ApplicationUser user)
    {
        var forum = _forumService.GetById(model.ForumId);

        return new Post
        {
            Title = model.Title,
            Content = model.Content,
            Created = DateTime.Now,
            User = user,
            Forum = forum,
            ImageUrl = model.PostImageUrl
        };
    }

    private IEnumerable<PostReplyModel> BuildPostReplies(IEnumerable<PostReply>? replies)
    {
        var webrootpathUserReply = "~/img/ProfileImages/";
        return replies.Select(reply => new PostReplyModel
        {
            Id = reply.Id,
            AuthorName = reply.User.UserName,
            AuthorId = reply.User.Id,
            AuthorImageUrl = reply.User.ProfileImageUrl,
            AuthorRating = reply.User.Rating,
            Created = reply.Created,
            ReplyContent = reply.Content,
            IsAuthorAdmin = IsAuthorAdmin(reply.User)
        });

    }
}
