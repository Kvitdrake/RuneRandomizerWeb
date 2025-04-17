using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Models;

namespace webb_tst_site3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Sphere> Spheres { get; set; }
        public DbSet<Rune> Runes { get; set; }
        public DbSet<RuneSphereDescription> RuneSphereDescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связей
            modelBuilder.Entity<RuneSphereDescription>()
                .HasOne(rsd => rsd.Rune)
                .WithMany(r => r.SphereDescriptions)
                .HasForeignKey(rsd => rsd.RuneId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RuneSphereDescription>()
                .HasOne(rsd => rsd.Sphere)
                .WithMany(s => s.RuneDescriptions)
                .HasForeignKey(rsd => rsd.SphereId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}