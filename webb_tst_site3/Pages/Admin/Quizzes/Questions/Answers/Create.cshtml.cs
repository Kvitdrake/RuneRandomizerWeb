// Pages/Admin/Quizzes/Answers/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions.Answers
{
    public class CreateModel : PageModel
    {
        public readonly AppDbContext _context;

        [BindProperty]
        public Answer Answer { get; set; }

        [BindProperty(SupportsGet = true)]
        public int QuizId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? QuestionId { get; set; }

        public SelectList Questions { get; set; }
        public SelectList Results { get; set; }
        public Models.Quiz Quiz { get; set; }

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Quiz = await _context.Quizzes.FindAsync(QuizId);
            if (Quiz == null)
            {
                return NotFound();
            }

            // ≈сли QuestionId не указан, показываем выбор вопроса
            if (!QuestionId.HasValue)
            {
                Questions = new SelectList(
                    await _context.Questions
                        .Where(q => q.QuizId == QuizId)
                        .OrderBy(q => q.Order)
                        .ToListAsync(),
                    "Id", "Text");
            }
            else
            {
                Answer = new Answer { QuestionId = QuestionId.Value };
            }

            Results = new SelectList(
                await _context.Results
                    .Where(r => r.QuizId == QuizId)
                    .OrderBy(r => r.Name)
                    .ToListAsync(),
                "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSelectListsAsync();
                return Page();
            }

            _context.Answers.Add(Answer);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { QuizId, QuestionId = Answer.QuestionId });
        }

        private async Task LoadSelectListsAsync()
        {
            Quiz = await _context.Quizzes.FindAsync(QuizId);

            if (!QuestionId.HasValue)
            {
                Questions = new SelectList(
                    await _context.Questions
                        .Where(q => q.QuizId == QuizId)
                        .OrderBy(q => q.Order)
                        .ToListAsync(),
                    "Id", "Text");
            }

            Results = new SelectList(
                await _context.Results
                    .Where(r => r.QuizId == QuizId)
                    .OrderBy(r => r.Name)
                    .ToListAsync(),
                "Id", "Name");
        }
    }
}