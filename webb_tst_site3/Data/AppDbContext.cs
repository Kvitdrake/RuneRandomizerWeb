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
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<UserQuizAnswer> UserQuizAnswers { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<SettingGroup> SettingGroups { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<SettingHistory> SettingHistory { get; set; }
        public DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // RuneSphereDescription
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

            modelBuilder.Entity<RuneSphereDescription>()
                .HasIndex(rsd => new { rsd.RuneId, rsd.SphereId })
                .IsUnique();

            // Quiz
            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Results)
                .WithOne(r => r.Quiz)
                .HasForeignKey(r => r.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question
            modelBuilder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Answer
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Result)
                .WithMany()
                .HasForeignKey(a => a.ResultId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserQuizAnswer
            modelBuilder.Entity<UserQuizAnswer>()
                .HasOne(uqa => uqa.Quiz)
                .WithMany()
                .HasForeignKey(uqa => uqa.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserQuizAnswer>()
                .HasOne(uqa => uqa.Question)
                .WithMany()
                .HasForeignKey(uqa => uqa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserQuizAnswer>()
                .HasOne(uqa => uqa.Answer)
                .WithMany()
                .HasForeignKey(uqa => uqa.AnswerId)
                .OnDelete(DeleteBehavior.Restrict);

            // SettingHistory (если присутствует ChangedByUserId, возможно нужен User)
            modelBuilder.Entity<SettingHistory>()
                .HasOne<Models.User>()
                .WithMany()
                .HasForeignKey(sh => sh.ChangedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Article
            modelBuilder.Entity<Article>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Article>()
                .Property(a => a.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();

            // Feedback
            modelBuilder.Entity<Feedback>()
                .Property(f => f.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}