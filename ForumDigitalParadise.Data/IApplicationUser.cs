using ForumDigitalParadise.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data
{
    public interface IApplicationUser
    {
        ApplicationUser GetById(string id); 
        IEnumerable<ApplicationUser> GetAll();

        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task Update(ApplicationUser user);
        Task Delete(ApplicationUser user);
        Task SetProfileImageUrl (string id, Uri uri);
        Task UpdateUserRating(string userId, Type type);
        Task UpdateUserPostRating(string userId, Type type);
        Task UpdateUserCommentRating(string userId, Type type);
    }
}
