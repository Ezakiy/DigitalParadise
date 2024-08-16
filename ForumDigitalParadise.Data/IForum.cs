using ForumDigitalParadise.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data
{
    public interface IForum
    {
        Forum GetById(int id);
        IEnumerable<Forum> GetAllForums();
        IEnumerable<ApplicationUser> GetAllActiveUsers(int id);
        IEnumerable<Forum> GetFilteredForums(int forumId, string searchQuery);
        IQueryable<Forum> GetAll();
        IEnumerable<Forum> SearchForums(string searchQuery);
        Task Create(Forum forum);
        Task Delete(int forumId);
        Task UpdateForum(Forum forum);
        bool HasRecentPost(int id);
    }
}
