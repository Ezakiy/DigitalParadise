using System.Threading.Tasks;
using ForumDigitalParadise.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ForumDigitalParadise.Data.Services
{
    public class PostReplyService : IPostReply
    {
        private readonly ApplicationDbContext _context;

        public PostReplyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddReply(PostReply reply)
        {
            _context.PostReplies.Add(reply);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<PostReply> GetRepliesByUserId(string userId)
        {
            return _context.PostReplies.Where(reply => reply.UserId == userId).ToList();
        }

        public async Task Delete(int id)
        {
            var reply = GetById(id);
            _context.Remove(reply);
            await _context.SaveChangesAsync();
        }

        public async Task Edit(int id, string message)
        {
            var reply = GetById(id);
            await _context.SaveChangesAsync();
            _context.Update(reply);
            await _context.SaveChangesAsync();
        }

        public PostReply GetById(int id)
        {
            return _context.PostReplies
                .Include(r => r.Post)
                .ThenInclude(post => post.Forum)
                .Include(r => r.Post)
                .ThenInclude(post => post.User).First(r => r.Id == id);
        }

        public PostReply GetLatestReplyForPost(int postId)
        {
            return _context.PostReplies
                .Where(pr => pr.PostId == postId)
                .OrderByDescending(pr => pr.Created)
                .FirstOrDefault();
        }
    }
}
