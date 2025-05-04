using Microsoft.EntityFrameworkCore;
using NpuApi.Models;

namespace NpuApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Creation> Creations { get; set; }
        public DbSet<CreationScore> CreationScores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                // Seed data as an example
                entity.HasData(new User
                {
                    Id = new Guid("9ecbfde3-df9e-4e60-a220-558609f1fe56"),
                    Username = "admin",
                    Email = "admin@oneringtorulethemallinnarnia.com",
                    CreatedAt = new DateTime(2025, 5, 2, 14, 46, 51, 542, DateTimeKind.Utc) 
                });
            });

            modelBuilder.Entity<CreationScore>(entity =>
            {
                entity.HasIndex(e => new { e.CreationId, e.UserId }).IsUnique();

                entity.HasOne(d => d.Creation)
                    .WithMany(p => p.CreationScores)
                    .HasForeignKey(d => d.CreationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}