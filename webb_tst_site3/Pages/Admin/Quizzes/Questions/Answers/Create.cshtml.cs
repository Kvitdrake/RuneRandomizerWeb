using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions.Answers
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Answer Answer { get; set; }
        public Question Question { get; set; }
        public SelectList Results { get; set; }
        public int QuizId { get; set; }

        public async Task<IActionResult> OnGetAsync(int questionId)
        {
            Question = await _context.Questions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (Question == null)
            {
                return NotFound();
            }

            QuizId = Question.QuizId;
            Answer = new Answer { QuestionId = questionId };

            Results = new SelectList(
                await _context.Results
                    .Where(r => r.QuizId == QuizId)
                    .ToListAsync(),
                "Id", "Name");

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid && Answer == null)
            {
                Question = await _context.Questions
                    .Include(q => q.Quiz)
                    .FirstOrDefaultAsync(q => q.Id == Answer.QuestionId);

                Results = new SelectList(
                    await _context.Results
                        .Where(r => r.QuizId == Question.QuizId)
                        .ToListAsync(),
                    "Id", "Name");

                return Page();
            }

            _context.Answers.Add(Answer);
            await _context.SaveChangesAsync();

            // Редирект на страницу ответов конкретного вопроса
            return RedirectToPage("./Index", new { questionId = Answer.QuestionId });
        }
    }
}