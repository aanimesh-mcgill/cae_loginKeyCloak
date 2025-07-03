using Microsoft.EntityFrameworkCore;
using LoginSystem.API.Models;

namespace LoginSystem.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.AdUsername).IsUnique();
                entity.HasIndex(e => e.Email);
                entity.Property(e => e.Role).HasDefaultValue("Viewer");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // LoginHistory configuration
            modelBuilder.Entity<LoginHistory>(entity =>
            {
                entity.HasIndex(e => e.LoginTimeUtc);
                entity.HasIndex(e => e.Username);
                entity.Property(e => e.LoginTimeUtc).HasDefaultValueSql("GETUTCDATE()");
                
                // Relationship with User
                entity.HasOne(e => e.User)
                      .WithMany(u => u.LoginHistories)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Content configuration
            modelBuilder.Entity<Content>(entity =>
            {
                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.IsPublished);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.IsPublished).HasDefaultValue(false);
            });

            // Document configuration
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasIndex(e => e.CreatedAt);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Seed data for demonstration
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed some sample content
            modelBuilder.Entity<Content>().HasData(
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "Welcome to the Login System",
                    Body = "This is a sample content item that demonstrates the role-based access control system. Viewers can see this content, editors can modify it, and admins have full control.",
                    CreatedBy = "system",
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Content
                {
                    Id = Guid.NewGuid(),
                    Title = "System Documentation",
                    Body = "This content is only visible to authenticated users. Different roles have different levels of access to content management features.",
                    CreatedBy = "system",
                    IsPublished = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
} 