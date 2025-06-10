using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using webb_tst_site3.Data;

namespace webb_tst_site3.Pages.Admin.Articles
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        [BindProperty] public Article Article { get; set; }
        public SelectList? Parents { get; set; }

        public EditModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context; _environment = environment;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Article = await _context.Articles.FindAsync(id)
                ?? throw new Exception("Not found");
            Parents = new SelectList(_context.Articles.Where(a => a.Id != id), "Id", "Title", Article.ParentId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            /*if (!ModelState.IsValid)
            {
                Parents = new SelectList(_context.Articles.Where(a => a.Id != Article.Id), "Id", "Title", Article.ParentId);
                return Page();
            }*/

            var existing = await _context.Articles.FindAsync(Article.Id);
            if (existing == null) return NotFound();

            existing.Title = Article.Title;
            existing.Description = Article.Description;
            existing.Url = Article.Url;
            existing.Hashtags = Article.Hashtags;
            existing.IsPublished = Article.IsPublished;
            existing.ParentId = Article.ParentId;

            if (Article.ImageFile != null && Article.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "uploads/articles");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(Article.ImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await Article.ImageFile.CopyToAsync(stream);
                existing.ImageUrl = "/uploads/articles/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}