using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Quizzes
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Models.Quiz> Quizzes { get; set; } = new List<Models.Quiz>();

        public async Task OnGetAsync()
        {
            Quizzes = await _context.Quizzes
                .AsNoTracking()
                .OrderBy(q => q.Title)
                .ToListAsync();
        }
    }
}