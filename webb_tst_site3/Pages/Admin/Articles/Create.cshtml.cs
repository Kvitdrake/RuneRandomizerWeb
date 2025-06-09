using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using webb_tst_site3.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Linq;
using webb_tst_site3.Data;

namespace webb_tst_site3.Pages.Admin.Articles
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public Article Article { get; set; }

        public SelectList ParentArticles { get; set; }

        public CreateModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public void OnGet()
        {
            ParentArticles = new SelectList(_context.Articles.Where(a => a.ParentId == null).ToList(), "Id", "Title");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ParentArticles = new SelectList(_context.Articles.Where(a => a.ParentId == null).ToList(), "Id", "Title");
                return Page();
            }

            if (Article.ImageFile != null && Article.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads/articles");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Article.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Article.ImageFile.CopyToAsync(stream);
                }

                Article.ImageUrl = "/uploads/articles/" + fileName;
            }

            _context.Articles.Add(Article);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}