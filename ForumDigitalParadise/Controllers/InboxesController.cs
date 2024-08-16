using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Models.Followers;
using ForumDigitalParadise.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Controllers
{
    [Authorize]
    public class InboxesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFollower _followerService;
        private readonly IApplicationUser _applicationUserService;

        public InboxesController(UserManager<ApplicationUser> userManager, IFollower followerService, IApplicationUser applicationUserService)
        {
            _userManager = userManager;
            _followerService = followerService;
            _applicationUserService = applicationUserService;
        }

        [HttpGet]
        public async Task<IActionResult> MyFollowers()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound();
            }

            var followers = await _followerService.GetFollowersAsync(currentUser.Id);

            var followerViewModels = followers.Select(f => new FollowerViewModel
            {
                FollowerId = f.FollowerId,
                FollowerName = _applicationUserService.GetById(f.FollowerId).DisplayName,
                FollowerProfileImageUrl = _applicationUserService.GetById(f.FollowerId).ProfileImageUrl
            }).ToList();

            var webrootpathUser = "~/img/ProfileImages/";

            var model = new MyFollowersModel
            {
                UserId = currentUser.Id,
                DisplayName = currentUser.DisplayName,
                ProfileImageUrl = string.IsNullOrEmpty(currentUser.ProfileImageUrl) ? null : webrootpathUser + currentUser.ProfileImageUrl,
                Bio = currentUser.UserDescription,
                BannerImageUrl = currentUser.BannerImageUrl,
                Followers = followerViewModels
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadFollowers()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            var followers = await _followerService.GetFollowersAsync(currentUser.Id);
            var followerViewModels = followers.Select(f => new FollowerViewModel
            {
                FollowerId = f.FollowerId,
                FollowerName = _applicationUserService.GetById(f.FollowerId).DisplayName,
                FollowerProfileImageUrl = _applicationUserService.GetById(f.FollowerId).ProfileImageUrl
            }).ToList();

            var model = new MyFollowersModel
            {
                UserId = currentUser.Id,
                Followers = followerViewModels
            };

            return PartialView("_FollowersPartial", model);
        }
    }
}
