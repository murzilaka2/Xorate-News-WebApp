using Microsoft.EntityFrameworkCore;
using Xorate.Models;

namespace Xorate.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> context) : base(context)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ShortPost> ShortPosts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>().HasIndex(e => e.Path).IsUnique();
            modelBuilder.Entity<Post>().Property(u => u.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Post>().Property(u => u.Likes).HasDefaultValue(0);
            modelBuilder.Entity<Post>().Property(u => u.Views).HasDefaultValue(1);
            modelBuilder.Entity<Comment>().Property(u => u.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Review>().Property(u => u.CreatedDate).HasDefaultValueSql("GETDATE()");
        }
    }
}
