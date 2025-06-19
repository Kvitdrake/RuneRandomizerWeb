// IndexModel.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
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

        public List<Article> ArticlesTree { get; set; } = new();

        public async Task OnGetAsync()
        {
            var allArticles = await _context.Articles
                .Include(a => a.Children)
                .AsNoTracking()
                .ToListAsync();

            ArticlesTree = allArticles
                .Where(a => a.ParentId == null)
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Title)
                .ToList();

            foreach (var root in ArticlesTree)
            {
                FillChildren(root, allArticles);
            }
        }

        private void FillChildren(Article parent, List<Article> allArticles)
        {
            parent.Children = allArticles
                .Where(a => a.ParentId == parent.Id)
                .OrderBy(a => a.DisplayOrder)
                .ThenBy(a => a.Title)
                .ToList();

            foreach (var child in parent.Children)
            {
                FillChildren(child, allArticles);
            }
        }
    }
}