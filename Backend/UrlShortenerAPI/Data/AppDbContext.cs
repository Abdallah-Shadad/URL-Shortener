using Microsoft.EntityFrameworkCore;
using UrlShortenerAPI.Models;

namespace UrlShortenerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            });

            // ShortenedUrl Configuration
            modelBuilder.Entity<ShortenedUrl>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.HasIndex(s => s.Code).IsUnique();
                entity.Property(s => s.LongUrl).IsRequired();

                // Relationship: User -> ShortenedUrls (1:N)
                entity.HasOne(s => s.User)
                      .WithMany(u => u.ShortenedUrls)
                      .HasForeignKey(s => s.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Analytics Configuration
            modelBuilder.Entity<Analytics>(entity =>
            {
                entity.HasKey(a => a.Id);

                // Relationship: ShortenedUrl -> Analytics (1:N)
                entity.HasOne(a => a.ShortenedUrl)
                      .WithMany(s => s.Analytics)
                      .HasForeignKey(a => a.ShortenedUrlId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ShortenedUrl> ShortenedUrls { get; set; }
        public DbSet<Analytics> Analytics { get; set; }
    }
}
