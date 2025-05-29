using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webb_tst_site3.Models
{
    public class Quiz
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название обязательно")]
        public string Title { get; set; }

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublished { get; set; } = true;

        public List<Question> Questions { get; set; } = new();
        public List<Result> Results { get; set; } = new();

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}