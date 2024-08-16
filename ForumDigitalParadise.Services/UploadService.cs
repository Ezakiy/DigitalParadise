using ForumDigitalParadise.Data;
using Microsoft.AspNetCore.Http;
using System.Collections;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace ForumDigitalParadise.Service
{
    public class UploadService : IUpload
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UploadService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string?> UploadForumImage(IFormFile imageUploadForum)
        {
            try
            {
                if (imageUploadForum == null || imageUploadForum.Length == 0)
                    return null;

                var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "ForumImages");

                if (!Directory.Exists(uploadsFolderPath))
                    Directory.CreateDirectory(uploadsFolderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageUploadForum.FileName);
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUploadForum.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading Forum image: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> UploadPostImage(IFormFile imageUploadPost)
        {
            if (imageUploadPost == null || imageUploadPost.Length == 0)
                return null;

            try
            {
                var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "PostImages");

                if (!Directory.Exists(uploadsFolderPath))
                    Directory.CreateDirectory(uploadsFolderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageUploadPost.FileName);
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUploadPost.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading post image: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> UploadProfileImage(IFormFile imageUploadProfile)
        {
            if (imageUploadProfile == null || imageUploadProfile.Length == 0)
                return null;

            try
            {
              
                var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "ProfileImages");

              
                if (!Directory.Exists(uploadsFolderPath))
                    Directory.CreateDirectory(uploadsFolderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageUploadProfile.FileName);
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUploadProfile.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading profile image: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> UploadBannerImage(IFormFile imageUploadBanner)
        {
            if (imageUploadBanner == null || imageUploadBanner.Length == 0)
                return null;

            try
            {

                var uploadsFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", "BannerImages");


                if (!Directory.Exists(uploadsFolderPath))
                    Directory.CreateDirectory(uploadsFolderPath);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageUploadBanner.FileName);
                var filePath = Path.Combine(uploadsFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageUploadBanner.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading profile image: {ex.Message}");
                return null;
            }
        }

    }
}
