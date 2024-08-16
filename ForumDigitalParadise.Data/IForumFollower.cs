using ForumDigitalParadise.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data
{
    public interface IForumFollower
    {
        Task FollowForumAsync(string userId, int forumId);
        Task UnfollowForumAsync(string userId, int forumId);
        Task<bool> IsFollowingForumAsync(string userId, int forumId);
        Task<int> GetForumFollowersCountAsync(int forumId);
        Task<List<ForumFollower>> GetForumFollowersAsync(int forumId);
        Task<List<Forum>> GetFollowedForumsAsync(string userId);
    }
}
