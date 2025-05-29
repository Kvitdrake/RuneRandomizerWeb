using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webb_tst_site3.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [StringLength(255)]
        public string Url { get; set; }

        public string ImageUrl { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [StringLength(500)]
        public string Hashtags { get; set; }

        public int? ParentId { get; set; }
        public Article Parent { get; set; }

        public List<Article> Children { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Метод для получения списка хэштегов
        public List<string> GetHashtagsList()
        {
            if (string.IsNullOrWhiteSpace(Hashtags))
                return new List<string>();

            return Hashtags.Split(new[] { '#', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(t => t.Trim())
                          .Where(t => !string.IsNullOrEmpty(t))
                          .Distinct()
                          .ToList();
        }
    }
}