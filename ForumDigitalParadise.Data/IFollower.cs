using ForumDigitalParadise.Data.Models;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data
{
    public interface IFollower
    {
        Task FollowUserAsync(string followerId, string followeeId);
        Task UnfollowUserAsync(string followerId, string followeeId);
        Task<bool> IsFollowingAsync(string followerId, string followeeId);
        Task<bool> IsFollowerAsync(string followeeId, string followerId);
        Task<int> GetFollowersCountAsync(string userId);
        Task<int> GetFollowingCountAsync(string userId);
        Task<List<Follower>> GetFollowersAsync(string userId);
        Task<List<ApplicationUser>> GetFollowedUsersAsync(string userId);
    }
}
