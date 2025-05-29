using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Articles
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Article> Articles { get; set; }

        public async Task OnGetAsync()
        {
            Articles = await _context.Articles
                .Where(a => a.ParentId == null)
                .Include(a => a.Children)
                .ToListAsync();
        }
    }
}