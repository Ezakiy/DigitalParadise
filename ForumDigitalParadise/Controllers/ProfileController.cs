using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Data.Services;
using ForumDigitalParadise.Models.ApplicationUser;
using ForumDigitalParadise.Models.Post;
using ForumDigitalParadise.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ForumDigitalParadise.Controllers
{
   
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        private readonly IUpload _uploadService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPost _postService;
        private readonly IPostLike _postLikeService;
        private readonly IPostReply _postReplyService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager, IApplicationUser userService, IUpload uploadService, IWebHostEnvironment webHostEnvironment, IPost postService, IPostReply postReplyService, SignInManager<ApplicationUser> signInManager, IPostLike postLikeService)
        {
            _userManager = userManager;
            _userService = userService;
            _uploadService = uploadService;
            _webHostEnvironment = webHostEnvironment;
            _postService = postService;
            _postReplyService = postReplyService;
            _signInManager = signInManager;
            _postLikeService = postLikeService;
        }
        [Authorize]
        public IActionResult Settings(string id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = _userManager.GetRolesAsync(user).Result;
            var webrootpathUser = "~/img/ProfileImages/";
            var webrootpathBanner = "~/img/BannerImages/";
            var model = new SettingsModel()
            {
                UserId = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName, 
                Email = user.Email,
                ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ? "~/img/users/default_image.jpg" : webrootpathUser + user.ProfileImageUrl,
                BannerImageUrl = webrootpathBanner + user.BannerImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin"),
                IsActive = user.IsActive
            };

            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string UserName, string DisplayName, string UserDescription)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (!string.IsNullOrEmpty(DisplayName))
            {
                user.DisplayName = DisplayName;
            }

            if (!string.IsNullOrEmpty(UserDescription))
            {
                user.UserDescription = UserDescription;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { success = true });
            }

            return BadRequest(new { success = false, errors = result.Errors });
        }

        public async Task<IActionResult> Overview(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isOwner = currentUser != null && currentUser.Id == user.Id;

            var userRoles = await _userManager.GetRolesAsync(user);

            var webrootpathUser = "~/img/ProfileImages/";
            var webrootpathBanner = "~/img/BannerImages/";
            var webrootpathPost = "~/img/PostImages/";

            var posts = _postService.GetPostsByUserId(user.Id);
            var replies = _postReplyService.GetRepliesByUserId(user.Id);

            var postListingModels = posts.Select(post => new PostListingModel
            {
                Id = post.Id,
                Title = post.Title,
                PostContent = post.Content,
                PostImageUrl = string.IsNullOrEmpty(post.ImageUrl) ? null : webrootpathPost + post.ImageUrl,
                Created = post.Created,
                Views = post.Views,
                RepliesCount = post.Replies.Count(),
                LikesCount = post.Likes.Count()
            }).ToList();

            var model = new ProfileModel
            {
                Id = user.Id,
                UserId = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                UserRating = user.Rating.ToString(),
                UserCommentRating = user.CommentRating.ToString(),
                UserPostRating = user.PostRating.ToString(),
                UserDescription = user.UserDescription,
                ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ? "~/img/users/default_image.jpg" : webrootpathUser + user.ProfileImageUrl,
                BannerImageUrl = string.IsNullOrEmpty(user.BannerImageUrl) ? null : webrootpathBanner + user.BannerImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin"),
                Posts = postListingModels,
                Replies = replies,
                IsOwner = isOwner
            };

            return View(model);
        }


        public async Task<IActionResult> Comments(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isOwner = currentUser != null && currentUser.Id == user.Id;

            var userRoles = await _userManager.GetRolesAsync(user);

            var webrootpathUser = "~/img/ProfileImages/";
            var webrootpathBanner = "~/img/BannerImages/";
            var webrootpathPost = "~/img/PostImages/";

            var posts = _postService.GetPostsByUserId(user.Id);
            var replies = _postReplyService.GetRepliesByUserId(user.Id);

            var postListingModels = posts.Select(post => new PostListingModel
            {
                Id = post.Id,
                Title = post.Title,
                PostContent = post.Content,
                PostImageUrl = string.IsNullOrEmpty(post.ImageUrl) ? null : webrootpathPost + post.ImageUrl,
                Created = post.Created,
                Views = post.Views,
                RepliesCount = post.Replies.Count()
            }).ToList();

            var model = new ProfileModel
            {
                Id = user.Id,
                UserId = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                UserRating = user.Rating.ToString(),
                UserCommentRating = user.CommentRating.ToString(),
                UserPostRating = user.PostRating.ToString(),
                UserDescription = user.UserDescription,
                ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ? "~/img/users/default_image.jpg" : webrootpathUser + user.ProfileImageUrl,
                BannerImageUrl = string.IsNullOrEmpty(user.BannerImageUrl) ? null : webrootpathBanner + user.BannerImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin"),
                Posts = postListingModels,
                Replies = replies,
                IsOwner = isOwner
            };

            return View(model);
        }

        public async Task<IActionResult> Posts(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var isOwner = currentUser != null && currentUser.Id == user.Id;

            var userRoles = await _userManager.GetRolesAsync(user);

            var webrootpathUser = "~/img/ProfileImages/";
            var webrootpathBanner = "~/img/BannerImages/";
            var webrootpathPost = "~/img/PostImages/";

            var posts = _postService.GetPostsByUserId(user.Id);
            var replies = _postReplyService.GetRepliesByUserId(user.Id);

            var postListingModels = posts.Select(post => new PostListingModel
            {
                Id = post.Id,
                Title = post.Title,
                PostContent = post.Content,
                PostImageUrl = string.IsNullOrEmpty(post.ImageUrl) ? null : webrootpathPost + post.ImageUrl,
                Created = post.Created,
                Views = post.Views,
                RepliesCount = post.Replies.Count()
            }).ToList();

            var model = new ProfileModel
            {
                Id = user.Id,
                UserId = user.Id,
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                UserRating = user.Rating.ToString(),
                UserCommentRating = user.CommentRating.ToString(),
                UserPostRating = user.PostRating.ToString(),
                UserDescription = user.UserDescription,
                ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ? "~/img/users/default_image.jpg" : webrootpathUser + user.ProfileImageUrl,
                BannerImageUrl = string.IsNullOrEmpty(user.BannerImageUrl) ? null : webrootpathBanner + user.BannerImageUrl,
                MemberSince = user.MemberSince,
                IsAdmin = userRoles.Contains("Admin"),
                Posts = postListingModels,
                Replies = replies,
                IsOwner = isOwner
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var userId = _userManager.GetUserId(User);
                    var imagePath = await _uploadService.UploadProfileImage(file);

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        user.ProfileImageUrl = imagePath;
                        await _userManager.UpdateAsync(user);

                        TempData["SuccessMessage"] = "Profile image uploaded successfully.";
                        return RedirectToAction("Overview", "Profile", new { id = user.Id });
                    }
                }
                TempData["ErrorMessage"] = "Oops! Something went wrong.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while uploading the profile image.";
            }
            return RedirectToAction("Overview", "Profile", new { id = _userManager.GetUserId(User) });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadProfileImageSettings(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var userId = _userManager.GetUserId(User);
                    var imagePath = await _uploadService.UploadProfileImage(file);

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        user.ProfileImageUrl = imagePath;
                        await _userManager.UpdateAsync(user);

                        TempData["SuccessMessage"] = "Profile image uploaded successfully.";
                        return RedirectToAction("Settings", "Profile", new { id = user.Id });
                    }
                }
                TempData["ErrorMessage"] = "Oops! Something went wrong.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while uploading the profile image.";
            }
            return RedirectToAction("Settings", "Profile", new { id = _userManager.GetUserId(User) });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UploadBannerImage(IFormFile bannerImage)
        {
            if (bannerImage != null && bannerImage.Length > 0)
            {
                var userId = _userManager.GetUserId(User);
                var imagePath = await _uploadService.UploadBannerImage(bannerImage);

                if (!string.IsNullOrEmpty(imagePath))
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    user.BannerImageUrl = imagePath;
                    await _userManager.UpdateAsync(user);

                    TempData["SuccessMessage"] = "Banner image uploaded successfully.";
                    return RedirectToAction("Overview", new { id = user.Id });
                }
            }
            TempData["ErrorMessage"] = "Oops! Something went wrong.";
            return RedirectToAction("Overview", new { id = _userManager.GetUserId(User) });
        }
    }
}
