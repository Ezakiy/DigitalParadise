using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Service
{
    public class ForumFollowerService : IForumFollower
    {
        private readonly ApplicationDbContext _context;

        public ForumFollowerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task FollowForumAsync(string userId, int forumId)
        {
            var follow = new ForumFollower
            {
                UserId = userId,
                ForumId = forumId,
                FollowingDate = DateTime.Now
            };

            _context.ForumFollowers.Add(follow);
            await _context.SaveChangesAsync();
        }

        public async Task UnfollowForumAsync(string userId, int forumId)
        {
            var follow = await _context.ForumFollowers
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ForumId == forumId);

            if (follow != null)
            {
                _context.ForumFollowers.Remove(follow);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFollowingForumAsync(string userId, int forumId)
        {
            return await _context.ForumFollowers
                .AnyAsync(f => f.UserId == userId && f.ForumId == forumId);
        }

        public async Task<int> GetForumFollowersCountAsync(int forumId)
        {
            return await _context.ForumFollowers
                .CountAsync(f => f.ForumId == forumId);
        }

        public async Task<List<ForumFollower>> GetForumFollowersAsync(int forumId)
        {
            return await _context.ForumFollowers
                .Where(f => f.ForumId == forumId)
                .ToListAsync();
        }

        public async Task<List<Forum>> GetFollowedForumsAsync(string userId)
        {
            var followedForums = await _context.ForumFollowers
                .Where(ff => ff.UserId == userId)
                .Include(ff => ff.Forum)
                .Select(ff => ff.Forum)
                .ToListAsync();

            return followedForums;
        }

    }
}
