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
            // Настройка связей для RuneSphereDescription
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

            // Настройка связей для Quiz
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

            // Настройка связей для Question
            modelBuilder.Entity<Question>()
                .HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка связей для Answer
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Result)
                .WithMany()
                .HasForeignKey(a => a.ResultId)
                .OnDelete(DeleteBehavior.Restrict);

            // Настройка связей для UserQuizAnswer
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

            // Настройка необязательных полей
            modelBuilder.Entity<Result>()
                .Property(r => r.ImageUrl)
                .IsRequired(false);

            // Уникальные индексы для улучшения производительности
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<RuneSphereDescription>()
                .HasIndex(rsd => new { rsd.RuneId, rsd.SphereId })
                .IsUnique();

            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasOne(r => r.Quiz)
                    .WithMany(q => q.Results)
                    .HasForeignKey(r => r.QuizId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(r => r.ImageUrl).IsRequired(false);
                entity.Navigation(r => r.Quiz).AutoInclude(false); // Отключаем автоматическую загрузку
            });

            modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Result)
                .WithMany()
                .HasForeignKey(a => a.ResultId);

            modelBuilder.Entity<Article>()
        .HasOne(a => a.Parent)
        .WithMany(a => a.Children)
        .HasForeignKey(a => a.ParentId)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Article>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<Article>()
                .Property(a => a.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}