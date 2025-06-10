using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Models;
using Microsoft.AspNetCore.Mvc;
using webb_tst_site3.Data;

namespace webb_tst_site3.Pages.Admin.Articles
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Article> ArticlesTree { get; set; } = new();

        public async Task OnGetAsync()
        {
            var all = await _context.Articles
                .Include(a => a.Children)
                .AsNoTracking()
                .ToListAsync();

            ArticlesTree = all.Where(a => a.ParentId == null)
                .OrderBy(a => a.Title)
                .ToList();
            foreach (var root in ArticlesTree)
                FillChildren(root, all);
        }

        private void FillChildren(Article parent, List<Article> all)
        {
            parent.Children = all.Where(a => a.ParentId == parent.Id)
                .OrderBy(a => a.Title)
                .ToList();
            foreach (var ch in parent.Children)
                FillChildren(ch, all);
        }
    }
}