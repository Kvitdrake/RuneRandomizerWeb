using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions.Answers
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public Question Question { get; set; }
        public List<Answer> Answers { get; set; }
        public int QuizId { get; set; }

        public async Task OnGetAsync(int questionId)
        {
            Question = await _context.Questions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            QuizId = Question.QuizId;

            Answers = await _context.Answers
                .Include(a => a.Result)
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }
    }
}