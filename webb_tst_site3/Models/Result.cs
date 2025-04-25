// Models/Result.cs
using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class Result
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }

        public int QuizId { get; set; }

        // Убираем Required для навигационного свойства
        public Quiz? Quiz { get; set; }
    }
}