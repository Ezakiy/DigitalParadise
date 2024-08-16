using ForumDigitalParadise.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data
{
    public interface ISwitch
    {
        Task<bool> SwitchAccountAsync(string currentUserId, string newAccountId);
        Task<List<ApplicationUser>> GetUserAccountsAsync(string userId);
        Task<bool> AddUserAccountAsync(string primaryUserId, string newAccountEmail, string newAccountPassword);
    }

}
