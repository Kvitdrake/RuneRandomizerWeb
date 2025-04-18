using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Question> Questions { get; set; } = new();
        public List<Result> Results { get; set; } = new();
        public bool IsPublished { get; set; } = true;
    }
}
