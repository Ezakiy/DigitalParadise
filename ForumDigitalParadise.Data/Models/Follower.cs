using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data.Models
{
    public class Follower
    {
        [Key, Column(Order = 0)]
        public string FollowerId { get; set; }

        [Key, Column(Order = 1)]
        public string FolloweeId { get; set; }
        public DateTime FollowingDate { get; set; }

        public virtual ApplicationUser FollowerUser { get; set; }
        public virtual ApplicationUser FolloweeUser { get; set; }
    }
}
