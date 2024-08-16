using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Service
{
    public class PostLikeService : IPostLike
    {
        private readonly ApplicationDbContext _context;
        public PostLikeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LikePostAsync(int postId, string userId)
        {
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                return;
            }

            var like = new Like
            {
                PostId = postId,
                UserId = userId,
                Created = DateTime.UtcNow
            };

            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
        }

        public async Task UnlikePostAsync(int postId, string userId)
        {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (like != null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetLikesCountAsync(int postId)
        {
            return await _context.Likes.CountAsync(l => l.PostId == postId);
        }

        public async Task<bool> IsPostLikedByUserAsync(int postId, string userId)
        {
            return await _context.Likes
                .AnyAsync(l => l.PostId == postId && l.UserId == userId);
        }
    }
}
