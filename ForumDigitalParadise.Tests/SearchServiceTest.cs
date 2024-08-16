using ForumDigitalParadise.Data;
using ForumDigitalParadise.Service;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace ForumDigitalParadise.Tests
{
    [TestFixture]
    public class Post_Service_Should
    {
        //    [Test]
        //    public void Return_Filtered_Results_Corresponding_To_Query()
        //    {
        //        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        //        .UseInMemoryDatabase(databaseName: "Search_Database").Options;


        //        using (var context = new ApplicationDbContext(options))
        //        {
        //            context.Forums.Add(new Data.Models.Forum
        //            {
        //                Id = 21
        //            });
        //            //all of these posts are assigned to the forum with the Id = 21
        //            context.Posts.Add(new Data.Models.Post {  
        //                Forum = context.Forums.Find(21),
        //                Id = 2123,
        //                Title = "FirstPostTest",
        //                Content = "I am the laziest person when it comes to work on my project..."
        //            });

        //            context.Posts.Add(new Data.Models.Post
        //            {
        //                Forum = context.Forums.Find(21),
        //                Id = 123124,
        //                Title = "now its the second test",
        //                Content = "FirstPostTest"
        //            });

        //            context.Posts.Add(new Data.Models.Post
        //            {
        //                Forum = context.Forums.Find(21),
        //                Id = -23123,
        //                Title = "third test yes thats right... third",
        //                Content = "FirstPostTest"
        //            });
        //        }

        //        using (var context = new ApplicationDbContext(options))
        //        {
        //            var postService = new PostService(context);
        //            var result = postService.GetFilteredPosts("FirstPostTest");
        //            var postCount = result.Count();

        //           Assert.Equals(3, postCount);
        //        }

        //    }
        //}
    }
}
