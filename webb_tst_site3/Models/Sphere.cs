using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Models
{
    public class Sphere
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название сферы обязательно")]
        [StringLength(100, ErrorMessage = "Название не должно превышать 100 символов")]
        public string Name { get; set; }

        // Сделаем описание необязательным
        public string? Description { get; set; }

        // Сделаем изображение необязательным
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<RuneSphereDescription> RuneDescriptions { get; set; } = new List<RuneSphereDescription>();
    }
}