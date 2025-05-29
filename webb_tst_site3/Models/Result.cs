using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public Quiz? Quiz { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}