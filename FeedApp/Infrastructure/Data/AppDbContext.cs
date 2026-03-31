using FeedApp.Domain.Entities;
using FeedApp.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FeedApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Feed> Feeds => Set<Feed>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new FeedConfiguration());
            modelBuilder.ApplyConfiguration(new ImageFeedConfiguration());
            modelBuilder.ApplyConfiguration(new VideoFeedConfiguration());
            modelBuilder.ApplyConfiguration(new LikeConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
        }
    }
}
