using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using webb_tst_site3.Data;

namespace webb_tst_site3.Pages.Admin.Articles
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        [BindProperty] public Article Article { get; set; } = null!;
        public SelectList? Parents { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ParentId { get; set; }

        public CreateModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context; _environment = environment;
        }

        public void OnGet()
        {
            Parents = new SelectList(_context.Articles, "Id", "Title", ParentId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            /*if (!ModelState.IsValid)
            {
                Parents = new SelectList(_context.Articles, "Id", "Title", ParentId);
                return Page();
            }*/

            if (Article.ImageFile != null && Article.ImageFile.Length > 0)
            {
                var uploads = Path.Combine(_environment.WebRootPath, "uploads/articles");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(Article.ImageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                    await Article.ImageFile.CopyToAsync(stream);
                Article.ImageUrl = "/uploads/articles/" + fileName;
            }
            Article.ParentId = ParentId;
            _context.Articles.Add(Article);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}