using ForumDigitalParadise.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data
{
    public interface INotification
    {
        Task CreateNotificationAsync(string userId, string message, string profileImageurl, string followerId);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
        Task MarkAsReadAsync(string userId);
        Task<int> GetUnreadNotificationsCountAsync(string userId);
    }
}
