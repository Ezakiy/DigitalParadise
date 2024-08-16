using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Data;
using ForumDigitalParadise.Models.Forum;
using ForumDigitalParadise.Models.Post;
using ForumDigitalParadise.Models.Composite;
using ForumDigitalParadise.Models.Reply;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ForumDigitalParadise.Service;
using ForumDigitalParadise.Data.Migrations;
using Microsoft.Extensions.Hosting;

public class ForumController : Controller
{
    private readonly IForum _forumService;
    private readonly IPost _postService;
    private readonly IPostReply _postReplyService;
    private readonly IUpload _uploadService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ForumController(IForum forumService, IPost postService, IUpload upload, UserManager<ApplicationUser> userManager, IPostReply postReplyService)
    {
        _forumService = forumService;
        _postService = postService;
        _uploadService = upload;
        _userManager = userManager;
        _postReplyService = postReplyService;
    }

    public IActionResult Index()
    {
        var webrootpath = "~/img/ForumImages/";
        var forums = _forumService.GetAllForums()
            .Select(forum => new ForumListingModel
            {
                Id = forum.Id,
                Name = forum.Title,
                Description = forum.Description,
                NumberOfPosts = forum.Posts?.Count() ?? 0,
                NumberOfUsers = _forumService.GetAllActiveUsers(forum.Id).Count(),
                ForumImageUrl = string.IsNullOrEmpty(forum.ImageUrl) ? "~/img/users/default_image.jpg" :  webrootpath + forum.ImageUrl,
                HasRecentPosts = _forumService.HasRecentPost(forum.Id)
            })
            .OrderBy(f => f.Name);

        var model = new ForumIndexModel
        {
            ForumList = forums
        };

        return View(model);
    }

    public IActionResult Topic(int id, string searchQuery)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var forum = _forumService.GetById(id);
        if (forum == null)
        {
            return NotFound();
        }

        var posts = _postService.GetFilteredPosts(id, searchQuery ?? "").ToList();

        PostReplyModel latestReply = null;

        if (posts.Any())
        {
            var lastPost = posts.Last();
            latestReply = GetLatestReplyForPost(lastPost);
        }

        var webrootpath = "~/img/PostImages/";
        var webrootpathUser = "~/img/ProfileImages/";

        var postListings = posts.Select(post => new PostListingModel
        {
            Id = post.Id,
            AuthorId = post.User.Id,
            AuthorRating = post.User.Rating,
            AuthorName = post.User.DisplayName,
            AuthorUserName = post.User.UserName,
            AuthorImageUrl = string.IsNullOrEmpty(post.User.ProfileImageUrl) ? "~/img/users/default_image.jpg" : webrootpathUser + post.User.ProfileImageUrl,
            IsAuthorAdmin = IsAuthorAdmin(post.User),
            Title = post.Title,
            DatePosted = post.Created.ToString(),
            RepliesCount = post.Replies.Count(),
            Views = post.Views,
            LatestReply = latestReply,
            Created = post.Created,
            Forum = BuildForumListing(post),
            PostImageUrl = string.IsNullOrEmpty(post.ImageUrl) ? null : webrootpath + post.ImageUrl,
            PostContent = post.Content,
            IsLikedByUser = HttpContext.User.Identity.IsAuthenticated && post.Likes.Any(like => like.UserId == _userManager.GetUserId(User))
        });

        var model = new CompositeViewForumModel
        {
            ForumTopicModel = new ForumTopicModel
            {
                Posts = postListings,
                Forum = BuildForumListing(forum)
            }
        };

        return View(model);
    }


    public bool IsAuthorAdmin(ApplicationUser user)
    {
        return _userManager.GetRolesAsync(user)
            .Result.Contains("Admin");
    }

    [HttpPost]
    public IActionResult Search(int forumId, string searchQuery)
    {
        return RedirectToAction("Topic", new { id = forumId, searchQuery });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        var model = new AddForumModel();
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddForum(AddForumModel model, IFormFile imageUploadForum)
    {
        string imageUrl = null;

        if (imageUploadForum != null && imageUploadForum.Length > 0)
        {
            imageUrl = await _uploadService.UploadForumImage(imageUploadForum);
        }

        var forum = new Forum
        {
            Title = model.Title,
            Description = model.Description,
            MiniDescription = model.MiniDescription,
            Created = DateTime.Now,
            ImageUrl = imageUrl
        };

        await _forumService.Create(forum);

        TempData["SuccessMessage"] = "Paradise added successfully.";
        return RedirectToAction("Index", "Forum");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id) 
    {       
            await _forumService.Delete(id);

        TempData["SuccessMessage"] = "Paradise deleted successfully.";
        return RedirectToAction("Index", "Forum");         
    }

    //da grab do conteudo
    [Authorize(Roles = "Admin")]
    public IActionResult Edit(int id)
    {
        var forum = _forumService.GetById(id);

        if (forum == null)
        {
            return RedirectToAction("Detail", "ErrorPage");
        }

        var webrootpathForum = "~/img/ForumImages/";

        var model = new EditForumModel
        {
            Id = forum.Id,
            Title = forum.Title,
            Description = forum.Description,
            MiniDescription = forum.MiniDescription,
            ForumImageUrl = webrootpathForum + forum.ImageUrl
         
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(EditForumModel model, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Detail", "ErrorPage");
        }

        var forum = _forumService.GetById(model.Id);

        if (forum == null)
        {
            return NotFound();
        }

        forum.Title = model.Title;
        forum.Description = model.Description;
        forum.MiniDescription = model.MiniDescription;

        if (file != null && file.Length > 0)
        {
            var forumImageUrl = await _uploadService.UploadForumImage(file);
            forum.ImageUrl = forumImageUrl;
            model.ForumImageUrl = forumImageUrl;
        }

        await _forumService.UpdateForum(forum);

        TempData["SuccessMessage"] = "Paradise edited successfully.";
        return RedirectToAction("Index", "Forum");
    }

    private PostReplyModel GetLatestReplyForPost(Post post)
    {
        var latestReply = _postReplyService.GetLatestReplyForPost(post.Id);

        if (latestReply == null)
        {
            return null;
        }

        var latestReplyUser = _userManager.FindByIdAsync(latestReply.User.Id).Result;

        return new PostReplyModel
        {
            AuthorId = latestReply.User.Id,
            AuthorName = latestReplyUser != null ? latestReplyUser.UserName : "Unknown",
            AuthorImageUrl = latestReplyUser != null ? latestReplyUser.ProfileImageUrl : "DefaultImageUrl",
            Created = latestReply.Created,
            ReplyContent = latestReply.Content
        };
    }

    private ForumListingModel BuildForumListing(Post post)
    {
        if (post.Forum == null)
        {
            return null;
        }

        var forum = post.Forum;
        return BuildForumListing(forum);
    }

    private ForumListingModel BuildForumListing(Forum forum)
    {
        var webrootpathForum = "~/img/ForumImages/";
        return new ForumListingModel
        {
            Id = forum.Id,
            Name = forum.Title,
            Description = forum.Description,
            MiniDescription = forum.MiniDescription,
            ForumImageUrl = webrootpathForum + forum.ImageUrl
        };
    }
}