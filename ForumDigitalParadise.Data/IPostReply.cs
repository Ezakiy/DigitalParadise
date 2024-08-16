using ForumDigitalParadise.Data.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ForumDigitalParadise.Data
{
    public interface IPostReply
    {
        PostReply GetById(int id);
        Task Edit(int id, string message);
        Task Delete (int id);
        Task AddReply(PostReply reply);
        PostReply GetLatestReplyForPost(int postId);
        IEnumerable<PostReply> GetRepliesByUserId(string userId);
    }
}
