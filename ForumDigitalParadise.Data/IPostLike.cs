using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data
{
    public interface IPostLike
    {
        Task LikePostAsync(int postId, string userId);
        Task UnlikePostAsync(int postId, string userId);
        Task<int> GetLikesCountAsync(int postId);
        Task<bool> IsPostLikedByUserAsync(int postId, string userId);
    }
}
