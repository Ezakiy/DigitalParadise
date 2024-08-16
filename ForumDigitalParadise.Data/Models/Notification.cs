using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }    
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } 
        public string ProfileImageUrl { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string FollowerId { get; set; }
    }
}
