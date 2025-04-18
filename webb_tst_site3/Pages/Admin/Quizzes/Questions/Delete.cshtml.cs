using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Question Question { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Question = await _context.Questions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (Question == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == Question.Id);

            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index", new { quizId = Question.QuizId });
        }
    }
}