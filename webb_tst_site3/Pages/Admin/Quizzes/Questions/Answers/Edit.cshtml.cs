using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions.Answers
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Answer Answer { get; set; }
        public SelectList Results { get; set; }
        public int QuizId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Answer = await _context.Answers
                .Include(a => a.Question)
                .ThenInclude(q => q.Quiz)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Answer == null)
            {
                return NotFound();
            }

            QuizId = Answer.Question.QuizId;
            Results = new SelectList(
                await _context.Results
                    .Where(r => r.QuizId == QuizId)
                    .ToListAsync(),
                "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Answer.Result");
            ModelState.Remove("Answer.Question");

            if (!ModelState.IsValid)
            {
                var answer = await _context.Answers
                    .Include(a => a.Question)
                    .ThenInclude(q => q.Quiz)
                    .FirstOrDefaultAsync(a => a.Id == Answer.Id);

                QuizId = answer?.Question?.QuizId ?? 0;

                Results = new SelectList(
                    await _context.Results
                        .Where(r => r.QuizId == QuizId)
                        .ToListAsync(),
                    "Id", "Name");

                return Page();
            }

            _context.Attach(Answer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnswerExists(Answer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../Index", new { quizId = QuizId });
        }

        private bool AnswerExists(int id)
        {
            return _context.Answers.Any(e => e.Id == id);
        }
    }
}