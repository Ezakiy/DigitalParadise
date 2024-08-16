using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ForumDigitalParadise.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ForumDigitalParadise.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostReply> PostReplies { get; set; }
        public DbSet<UserView> UserViews { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Follower> Followers { get; set; }
        public DbSet<ForumFollower> ForumFollowers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserView>()
                .HasKey(uv => new { uv.UserId, uv.PostId });

            modelBuilder.Entity<UserView>()
                .HasOne(uv => uv.User)
                .WithMany()
                .HasForeignKey(uv => uv.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserView>()
                .HasOne(uv => uv.Post)
                .WithMany()
                .HasForeignKey(uv => uv.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostReply>()
                .HasOne(pr => pr.Post)
                .WithMany(p => p.Replies)
                .HasForeignKey(pr => pr.PostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PostReply>()
                .HasOne(pr => pr.User)
                .WithMany()
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Follower>()
               .HasKey(f => new { f.FollowerId, f.FolloweeId });

            modelBuilder.Entity<Follower>()
                .HasOne(f => f.FollowerUser)
                .WithMany(u => u.Followees)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follower>()
                .HasOne(f => f.FolloweeUser)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}