using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages
{
    public class ArticlesModel : PageModel
    {
        private readonly AppDbContext _context;

        public ArticlesModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Article> Articles { get; set; } = new();

        public async Task OnGetAsync()
        {
            Articles = await _context.Articles
                .Include(a => a.Children)
                .Where(a => a.IsPublished)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}