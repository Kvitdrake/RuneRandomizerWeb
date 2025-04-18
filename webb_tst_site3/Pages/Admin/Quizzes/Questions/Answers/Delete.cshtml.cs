using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions.Answers
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Answer Answer { get; set; }
        public int QuizId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Answer = await _context.Answers
                .Include(a => a.Result)
                .Include(a => a.Question)
                .ThenInclude(q => q.Quiz)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Answer == null)
            {
                return NotFound();
            }

            QuizId = Answer.Question.QuizId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var answer = await _context.Answers.FindAsync(Answer.Id);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("../Index", new { quizId = QuizId });
        }
    }
}