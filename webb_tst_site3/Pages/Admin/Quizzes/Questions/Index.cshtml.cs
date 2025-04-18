using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public Models.Quiz Quiz { get; set; }
        public IList<Question> Questions { get; set; }

        public async Task OnGetAsync(int quizId)
        {
            Quiz = await _context.Quizzes.FindAsync(quizId);
            Questions = await _context.Questions
                .Include(q => q.Answers)
                .ThenInclude(a => a.Result)
                .Where(q => q.QuizId == quizId)
                .ToListAsync();
        }
    }
}