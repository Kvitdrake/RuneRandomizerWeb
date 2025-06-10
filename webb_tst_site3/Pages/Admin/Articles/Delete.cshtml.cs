using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Articles
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;
        public DeleteModel(AppDbContext context) => _context = context;

        [BindProperty] public Article? Article { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Article = await _context.Articles.FindAsync(id);
            if (Article == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Article == null) return NotFound();
            var art = await _context.Articles.FindAsync(Article.Id);
            if (art != null)
            {
                _context.Articles.Remove(art);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}