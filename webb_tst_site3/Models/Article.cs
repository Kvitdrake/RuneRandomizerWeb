using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace webb_tst_site3.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название статьи обязательно")]
        [StringLength(255, ErrorMessage = "Название не должно превышать 255 символов")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "URL обязательно")]
        [StringLength(255, ErrorMessage = "URL не должен превышать 255 символов")]
        [Url(ErrorMessage = "Некорректный URL")]
        public string Url { get; set; } = string.Empty;

        [Display(Name = "URL изображения")]
        public string? ImageUrl { get; set; }

        [NotMapped]
        [Display(Name = "Файл изображения")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Хэштеги")]
        [StringLength(500, ErrorMessage = "Хэштеги не должны превышать 500 символов")]
        public string? Hashtags { get; set; }

        [Display(Name = "Родительская статья")]
        public int? ParentId { get; set; }

        [Display(Name = "Родительская статья")]
        public Article? Parent { get; set; }

        [Display(Name = "Дочерние статьи")]
        public List<Article> Children { get; set; } = new();

        [Display(Name = "Дата создания")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Дата обновления")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Опубликовано")]
        public bool IsPublished { get; set; } = false;

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