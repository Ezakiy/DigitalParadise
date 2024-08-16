using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Service
{
    public class PostService : IPost
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(Post post)
        {
            _context.Add(post);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Post> GetPostsByUserId(string userId)
        {
            return _context.Posts.Where(post => post.User.Id == userId).ToList();
        }

        public async Task DeletePostAndRelatedRecords(int postId)
        {
            var post = await _context.Posts
                .Include(p => p.Replies)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post != null)
            {
                // Remove replies
                _context.PostReplies.RemoveRange(post.Replies);

                // Remove user views
                var userViews = _context.UserViews.Where(u => u.PostId == postId);
                _context.UserViews.RemoveRange(userViews);

                // Remove likes
                var postLikes = _context.Likes.Where(l => l.PostId == postId);
                _context.Likes.RemoveRange(postLikes);

                // Remove the post
                _context.Posts.Remove(post);

                await _context.SaveChangesAsync();
            }
        }


        public async Task UpdatePost(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Post> GetAll()
        {
            return _context.Posts
                .Include(post => post.User)
                .Include(post => post.Replies).ThenInclude(reply => reply.User)
                .Include(post => post.Forum)
                .AsQueryable();
        }
        public IEnumerable<Post> GetAllPosts()
        {
            return _context.Posts
                .Include(post => post.User)
                .Include(post => post.Replies).ThenInclude(reply => reply.User)
                .Include(post => post.Forum);
        }

        public Post GetById(int id)
        {
            return _context.Posts.Where(post => post.Id == id)
                .Include(post => post.Forum)
                .Include(post => post.User)
                .Include(post => post.Replies)
                .ThenInclude(reply => reply.User)
                .FirstOrDefault();
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Forum)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public IEnumerable<Post> GetFilteredPosts(int forumId, string searchQuery = "")
        {
            return _context.Posts
                .Where(p => p.Forum.Id == forumId &&
                            (string.IsNullOrEmpty(searchQuery) ||
                             p.Title.Contains(searchQuery) ||
                             p.Content.Contains(searchQuery)))
                .Include(p => p.User)
                .Include(p => p.Replies)
                .ThenInclude(r => r.User)
                .ToList();
        }

        public IEnumerable<Post> GetLatestPosts(int n)
        {
            return GetAllPosts().OrderByDescending(post => post.Created).Take(n);
        }

        public IEnumerable<Post> GetPostsByForum(int id)
        {
            return _context.Forums
                .Where(forum => forum.Id == id).First()
                .Posts;
        }

        public IEnumerable<Post> GetPostsByForum(string searchQuery)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasUserViewedPost(string userId, int postId)
        {
            return await _context.UserViews
                .AnyAsync(v => v.UserId == userId && v.PostId == postId);
        }

        public async Task RecordPostView(string userId, int postId)
        {
            if (!await HasUserViewedPost(userId, postId))
            {
                _context.UserViews.Add(new UserView
                {
                    UserId = userId,
                    PostId = postId,
                    ViewedAt = DateTime.UtcNow
                });

                var post = await _context.Posts.FindAsync(postId);
                if (post != null)
                {
                    post.Views++;
                }

                await _context.SaveChangesAsync();
            }
        }

        public IEnumerable<Post> SearchPosts(string searchQuery)
        {
            return GetAll().Where(post => EF.Functions.Like(post.Title, $"%{searchQuery}%")
                                       || EF.Functions.Like(post.Content, $"%{searchQuery}%")).ToList();
        }

        public IEnumerable<Post> GetPostsSorted(string sortBy)
        {
            var posts = GetAll().Include(post => post.Likes).AsEnumerable();

            switch (sortBy.ToLower())
            {
                case "views":
                    posts = posts.OrderByDescending(post => post.Views);
                    break;
                case "replies":
                    posts = posts.OrderByDescending(post => post.GetRepliesCount());
                    break;
                case "likes":
                    posts = posts.OrderByDescending(post => post.GetLikesCount());
                    break;
                case "Recent":
                    posts = posts.OrderByDescending(post => post.Created);
                    break;
                case "best":
                    posts = posts.OrderByDescending(post => post.Views + post.GetRepliesCount() + post.GetLikesCount());
                    break;
                default:
                    posts = posts.OrderByDescending(post => post.Created);
                    break;
            }

            return posts.ToList();
        }

    }
}
