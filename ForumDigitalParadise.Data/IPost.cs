using ForumDigitalParadise.Data.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ForumDigitalParadise.Data
{
    public interface IPost
    {
        Task Add(Post post);
        Task DeletePostAndRelatedRecords(int postId);
        Task UpdatePost(Post post);
        Post GetById(int id);
        Task<Post> GetByIdAsync(int id);
        IQueryable<Post> GetAll();
        IEnumerable<Post> GetAllPosts();
        IEnumerable<Post> GetFilteredPosts(int forumId, string searchQuery);
        IEnumerable<Post> GetPostsByForum(int id);
        IEnumerable<Post> GetLatestPosts(int forumId);
        IEnumerable<Post> GetPostsByUserId(string userId);
        IEnumerable<Post> GetPostsSorted(string sortBy);
        Task<bool> HasUserViewedPost(string userId, int postId);
        Task RecordPostView(string userId, int postId);
        IEnumerable<Post> SearchPosts(string searchQuery);
    }
}
