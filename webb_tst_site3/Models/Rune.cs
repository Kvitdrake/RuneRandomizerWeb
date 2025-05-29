using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webb_tst_site3.Models
{
    public class Rune
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название руны обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; }

        // Сделаем описание необязательным
        public string? BaseDescription { get; set; }

        // Сделаем изображение необязательным
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public int Order { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RuneSphereDescription> SphereDescriptions { get; set; } = new List<RuneSphereDescription>();
    }
}