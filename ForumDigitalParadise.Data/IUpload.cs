using System.Collections;
using Microsoft.AspNetCore.Http;

namespace ForumDigitalParadise.Data
{
    public interface IUpload
    {
        Task<string?> UploadProfileImage(IFormFile imageUploadUser);

        Task<string?> UploadPostImage(IFormFile imageUploadPost);
        Task<string?> UploadForumImage(IFormFile imageUploadForum);
        Task<string?> UploadBannerImage(IFormFile imageUploadBanner);

    }
}
