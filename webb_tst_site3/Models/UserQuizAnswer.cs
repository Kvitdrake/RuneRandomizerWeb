using System;
using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class UserQuizAnswer
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }  // Связь с пользователем

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int AnswerId { get; set; }

        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        // Навигационные свойства
        public Quiz Quiz { get; set; }
        public Question Question { get; set; }
        public Answer Answer { get; set; }
    }
}