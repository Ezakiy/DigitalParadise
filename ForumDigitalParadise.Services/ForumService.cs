using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ForumDigitalParadise.Service
{
    public class ForumService : IForum
    {
        private readonly ApplicationDbContext _context;
        private readonly IForumFollower _forumFollowerService;

        public ForumService(ApplicationDbContext context, IForumFollower forumFollowerService)
        {
            _context = context;
            _forumFollowerService = forumFollowerService;
        }

        public async Task Create(Forum forum)
        {
            _context.Add(forum);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var forum = GetById(id);

            if (forum != null)
            {
                foreach (var post in forum.Posts)
                {

                    var likes = _context.Likes.Where(like => like.PostId == post.Id);
                    _context.RemoveRange(likes);


                    var userViews = _context.UserViews.Where(uv => uv.PostId == post.Id);
                    _context.RemoveRange(userViews);

                    _context.RemoveRange(post.Replies);
                }

                _context.RemoveRange(forum.Posts);

                _context.Remove(forum);

                await _context.SaveChangesAsync();
            }
        }


        public IEnumerable<Forum> GetAllForums()
        {
            return _context.Forums.Include(forum => forum.Posts);
        }

        public IEnumerable<ApplicationUser> GetAllActiveUsers(int id)
        {
            var posts = GetById(id)?.Posts;

            if (posts != null && posts.Any())
            {
                var postUsers = posts.Select(post => post.User);
                var replyUsers = posts.SelectMany(post => post.Replies).Select(reply => reply.User);
                return postUsers.Union(replyUsers).Distinct();
            }

            return new List<ApplicationUser>();
        }

        public Forum GetById(int id)
        {
            var forum = _context.Forums.Where(f => f.Id == id)
                   .Include(f => f.Posts).ThenInclude(p => p.User)
                   .Include(f => f.Posts).ThenInclude(p => p.Replies).ThenInclude(r => r.User)
                   .FirstOrDefault();
            return forum;
        }

        public IEnumerable<Forum> GetFilteredForums(int forumId, string searchQuery)
        {
            var query = (searchQuery ?? "").ToLower();

            return _context.Forums
                           .Include(forum => forum.Posts)
                           .Where(forum => forum.Id == forumId && forum.Title.ToLower().Contains(query))
                           .ToList();
        }

        public bool HasRecentPost(int id)
        {
            const int hoursAgo = 72;
            var time = DateTime.Now.AddHours(hoursAgo);
            return GetById(id).Posts.Any(post => post.Created > time);
        }

        public async Task UpdateForum(Forum forum)
        {
            _context.Forums.Update(forum);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Forum> GetAll()
        {
            return _context.Forums.Include(forum => forum.Posts);
        }

        public IEnumerable<Forum> SearchForums(string searchQuery)
        {
            return GetAll().Where(forum => EF.Functions.Like(forum.Title, $"%{searchQuery}%")
                                        || EF.Functions.Like(forum.Description, $"%{searchQuery}%")).ToList();
        }      
    }
}
