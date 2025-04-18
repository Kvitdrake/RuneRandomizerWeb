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

            modelBuilder.Entity<Question>()
            .HasOne(q => q.Quiz)
            .WithMany(q => q.Questions)
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Result)
                .WithMany()
                .HasForeignKey(a => a.ResultId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<Answer>()
        .HasOne(a => a.Question)
        .WithMany(q => q.Answers)
        .HasForeignKey(a => a.QuestionId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Result)
                .WithMany()
                .HasForeignKey(a => a.ResultId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
        .HasOne(a => a.Question)
        .WithMany(q => q.Answers)
        .HasForeignKey(a => a.QuestionId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Result)
                .WithMany()
                .HasForeignKey(a => a.ResultId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Result>()
        .HasOne(r => r.Quiz)
        .WithMany(q => q.Results)
        .HasForeignKey(r => r.QuizId)
        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}