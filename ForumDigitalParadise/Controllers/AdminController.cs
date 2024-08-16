using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Data;
using ForumDigitalParadise.Models.ApplicationUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ForumDigitalParadise.Controllers
{

    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUser _userService;
        private readonly IUpload _uploadService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPost _postService;
        private readonly IPostReply _postReplyService;

        public AdminController(UserManager<ApplicationUser> userManager, IApplicationUser userService, IUpload uploadService, IWebHostEnvironment webHostEnvironment, IPost postService, IPostReply postReplyService)
        {
            _userManager = userManager;
            _userService = userService;
            _uploadService = uploadService;
            _webHostEnvironment = webHostEnvironment;
            _postService = postService;
            _postReplyService = postReplyService;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var webrootpathUser = "~/img/ProfileImages/";

            var profiles = _userService.GetAll()
                .OrderByDescending(user => user.Rating)
                .Select(user => new SettingsModel
                {
                    DisplayId = user.DisplayId,  
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl) ? "~/img/users/default_image.jpg" : webrootpathUser + user.ProfileImageUrl,
                    UserRating = user.Rating.ToString(),
                    MemberSince = user.MemberSince,
                    IsActive = user.IsActive,
                    IsAdmin = User.IsInRole("Admin")
                });

            var model = new ProfileListModel
            {
                Profiles = profiles
            };

            return View(model);
        }


        public IActionResult EditUser(string id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new SettingsModel
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                UserRating = user.Rating.ToString(),
                ProfileImageUrl = user.ProfileImageUrl,
                IsActive = user.IsActive,
                IsAdmin = User.IsInRole("Admin")
            };

            return View(model);
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(SettingsModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetById(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Rating = int.Parse(model.UserRating);
                user.IsActive = model.IsActive;

                _userService.Update(user);
                TempData["SuccessMessage"] = "Paradise added successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                return NotFound();
            }

            await _userService.Delete(user);
            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction("Index", "Admin");
        }
    }
}
