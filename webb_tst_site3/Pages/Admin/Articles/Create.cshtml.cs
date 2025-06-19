// CreateModel.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Articles
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Article Article { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public ContentType? Type { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ParentId { get; set; }

        public SelectList? Parents { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Article.ContentType = Type ?? ContentType.Article;
            Article.ParentId = ParentId;

            Parents = new SelectList(await _context.Articles
                .Where(a => a.ContentType == ContentType.Section)
                .ToListAsync(), "Id", "Title");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Parents = new SelectList(await _context.Articles
                    .Where(a => a.ContentType == ContentType.Section)
                    .ToListAsync(), "Id", "Title");
                return Page();
            }

            // Для разделов устанавливаем порядок в конец
            if (Article.ContentType == ContentType.Section)
            {
                var maxOrder = await _context.Articles
                    .Where(a => a.ParentId == Article.ParentId)
                    .MaxAsync(a => (int?)a.DisplayOrder) ?? -1;
                Article.DisplayOrder = maxOrder + 1;
            }

            _context.Articles.Add(Article);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}