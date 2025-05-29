using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace webb_tst_site3.Pages.Admin.Articles
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public Article Article { get; set; }

        [BindProperty]
        public bool DeleteImage { get; set; }

        public EditModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Article = await _context.Articles.FindAsync(id);

            if (Article == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingArticle = await _context.Articles.FindAsync(Article.Id);
            if (existingArticle == null)
            {
                return NotFound();
            }

            existingArticle.Title = Article.Title;
            existingArticle.Description = Article.Description;
            existingArticle.Url = Article.Url;
            existingArticle.Hashtags = Article.Hashtags;
            existingArticle.UpdatedAt = DateTime.UtcNow;

            // Обработка изображения
            if (DeleteImage && !string.IsNullOrEmpty(existingArticle.ImageUrl))
            {
                var filePath = Path.Combine(_environment.WebRootPath, existingArticle.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                existingArticle.ImageUrl = null;
            }

            if (Article.ImageFile != null && Article.ImageFile.Length > 0)
            {
                // Удаляем старое изображение
                if (!string.IsNullOrEmpty(existingArticle.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, existingArticle.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Сохраняем новое изображение
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads/articles");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Article.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Article.ImageFile.CopyToAsync(stream);
                }

                existingArticle.ImageUrl = "/uploads/articles/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}