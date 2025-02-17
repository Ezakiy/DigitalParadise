﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Data.Models
{
    public class Forum
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MiniDescription { get; set; }
        public DateTime Created { get; set; }
        public string? ImageUrl { get; set; }
        public virtual IEnumerable<Post>? Posts { get; set; }
    }
}
