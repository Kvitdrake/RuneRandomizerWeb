using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webb_tst_site3.Pages.Admin.Quizzes.Results
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public Models.Quiz Quiz { get; set; }
        public List<Models.Result> Results { get; set; }

        public async Task OnGetAsync(int quizId)
        {
            Quiz = await _context.Quizzes.FindAsync(quizId);
            Results = await _context.Results
                .Where(r => r.QuizId == quizId)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}