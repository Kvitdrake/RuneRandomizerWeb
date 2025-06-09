using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace webb_tst_site3.Pages
{
    public class ArticlesModel : PageModel
    {
        private readonly AppDbContext _context;

        public ArticlesModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Article> Articles { get; set; }

        public async Task OnGetAsync()
        {
            Articles = await _context.Articles
                .Where(a => a.IsPublished)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}