using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Service
{
    public class FollowerService : IFollower
    {
        private readonly ApplicationDbContext _context;
        private readonly INotification _notificationService;

        public FollowerService(ApplicationDbContext context, INotification notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task FollowUserAsync(string followerId, string followeeId)
        {
            var follow = new Follower
            {
                FollowerId = followerId,
                FolloweeId = followeeId,
                FollowingDate = DateTime.Now
            };

            _context.Followers.Add(follow);
            await _context.SaveChangesAsync();

            var follower = await _context.Users.FindAsync(followerId);
            var message = $"<a href='/Profile/Overview/{followerId}'>u/{follower.DisplayName}</a> Follows you.";
            var profileImageUrl = string.IsNullOrEmpty(follower.ProfileImageUrl)
                                  ? "/img/users/default_image.jpg"
                                  : $"/img/ProfileImages/{follower.ProfileImageUrl}";

            await _notificationService.CreateNotificationAsync(followeeId, message, profileImageUrl, followerId);
        }

        public async Task UnfollowUserAsync(string followerId, string followeeId)
        {
            var follow = await _context.Followers
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

            if (follow != null) 
            {
                _context.Followers.Remove(follow);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFollowingAsync(string followerId, string followeeId)
        {
            return await _context.Followers
                .AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
        }

        public async Task<bool> IsFollowerAsync(string followeeId, string followerId)
        {
            return await _context.Followers
                .AnyAsync(f => f.FolloweeId == followeeId && f.FollowerId == followerId);
        }

        public async Task<int> GetFollowersCountAsync(string userId)
        {
            return await _context.Followers
                .CountAsync(f => f.FolloweeId == userId);
        }

        public async Task<List<Follower>> GetFollowersAsync(string userId)
        {
            return await _context.Followers
                .Where(f => f.FolloweeId == userId)
                .ToListAsync();
        }

        public async Task<int> GetFollowingCountAsync(string userId)
        {
            return await _context.Followers.CountAsync(f => f.FollowerId == userId);
        }
        public async Task<List<ApplicationUser>> GetFollowedUsersAsync(string userId)
        {
            var followedUsers = await _context.Followers
                .Where(f => f.FollowerId == userId)
                .Include(f => f.FolloweeUser)
                .Select(f => f.FolloweeUser)
                .ToListAsync();

            return followedUsers;
        }
    }
}
