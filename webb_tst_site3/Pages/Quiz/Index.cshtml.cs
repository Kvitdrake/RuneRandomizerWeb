using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;

namespace webb_tst_site3.Pages.Quiz
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Models.Quiz> Quizzes { get; set; }

        public async Task OnGetAsync()
        {
            Quizzes = await _context.Quizzes
                .ToListAsync();
        }
    }
}