using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Service
{
    public class ApplicationUserService : IApplicationUser
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser GetById(string id)
        {
            return _context.ApplicationUsers.FirstOrDefault(user => user.Id == id);
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.ApplicationUsers;
        }

        public async Task Update(ApplicationUser user)
        {
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(ApplicationUser user)
        {

            var posts = _context.Posts.Where(p => p.UserId == user.Id);


            var postLikes = _context.Likes.Where(l => posts.Any(p => p.Id == l.PostId));
            _context.Likes.RemoveRange(postLikes);

            var userLikes = _context.Likes.Where(l => l.UserId == user.Id);
            _context.Likes.RemoveRange(userLikes);

            var postReplies = _context.PostReplies.Where(pr => posts.Any(p => p.Id == pr.PostId));
            _context.PostReplies.RemoveRange(postReplies);

            var userViews = _context.UserViews.Where(uv => uv.UserId == user.Id || posts.Any(p => p.Id == uv.PostId));
            _context.UserViews.RemoveRange(userViews);

            _context.Posts.RemoveRange(posts);

            var followers = _context.Followers.Where(f => f.FolloweeId == user.Id || f.FollowerId == user.Id);
            _context.Followers.RemoveRange(followers);

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserRating(string userId, Type type)
        {
            var user = GetById(userId);
            user.Rating = CalculateUserRating(type, user.Rating);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserPostRating(string userId, Type type)
        {
            var user = GetById(userId);
            user.PostRating = CalculateUserPostRating(type, user.PostRating);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserCommentRating(string userId, Type type)
        {
            var user = GetById(userId);
            user.CommentRating = CalculateUserCommentRating(type, user.CommentRating);
            await _context.SaveChangesAsync();
        }

        private int CalculateUserPostRating(Type type, int userRating)
        {
            var points = 0;

            if (type == typeof(Post))
            {
                points = 3;
            }

            return userRating + points;
        }

        private int CalculateUserCommentRating(Type type, int userRating)
        {
            var points = 0;

            if (type == typeof(PostReply))
            {
                points = 5;
            }

            return userRating + points;
        }

        private int CalculateUserRating(Type type, int userRating)
        {
            var points = 0;

            if(type == typeof(Post))
            {
                points = 3;
            }

            if(type == typeof(PostReply))
            {
                points = 5;
            }

            return userRating + points ;
        }

        public async Task SetProfileImageUrl(string id, Uri uri)
        {
            var user = GetById(id);
            user.ProfileImageUrl = uri.AbsoluteUri;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
