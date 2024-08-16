using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ForumDigitalParadise.Shared.Hubs;

namespace ForumDigitalParadise.Service
{
    public class NotificationService : INotification
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task CreateNotificationAsync(string userId, string message, string profileImageUrl, string followerId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new ArgumentException("userId doesn't exist.");
            }

            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false,
                ProfileImageUrl = profileImageUrl,
                FollowerId = followerId
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var notificationData = new
            {
                message = message,
                profileImageUrl = profileImageUrl,
                followerId = followerId
            };

            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", notificationData);
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(string userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            _context.Notifications.UpdateRange(unreadNotifications);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadNotificationsCountAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }
    }
}
