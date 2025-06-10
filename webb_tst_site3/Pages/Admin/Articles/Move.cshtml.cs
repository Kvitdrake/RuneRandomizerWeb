using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using webb_tst_site3.Data;

namespace webb_tst_site3.Pages.Admin.Articles
{
    public class MoveModel : PageModel
    {
        private readonly AppDbContext _context;
        public MoveModel(AppDbContext context) => _context = context;
        [BindProperty] public int Id { get; set; }
        [BindProperty] public int? NewParentId { get; set; }
        public SelectList? Parents { get; set; }
        public Article? Article { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Article = await _context.Articles.FindAsync(id);
            Id = id;
            Parents = new SelectList(_context.Articles.Where(a => a.Id != id), "Id", "Title");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var art = await _context.Articles.FindAsync(Id);
            if (art == null) return NotFound();
            art.ParentId = NewParentId;
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}