using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(AppDbContext context, ILogger<CreateModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Question Question { get; set; }

        public async Task<IActionResult> OnGetAsync(int quizId)
        {
            // Проверяем существование квиза
            var quizExists = await _context.Quizzes.AnyAsync(q => q.Id == quizId);
            if (!quizExists)
            {
                return NotFound();
            }

            Question = new Question { QuizId = quizId };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Проверяем существование квиза
                var quizExists = await _context.Quizzes.AnyAsync(q => q.Id == Question.QuizId);
                if (!quizExists)
                {
                    ModelState.AddModelError("", "Указанный квиз не существует");
                    return Page();
                }

                // Автоматическая нумерация
                if (Question.Order == 0)
                {
                    var maxOrder = await _context.Questions
                        .Where(q => q.QuizId == Question.QuizId)
                        .MaxAsync(q => (int?)q.Order) ?? 0;
                    Question.Order = maxOrder + 1;
                }

                _context.Questions.Add(Question);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Index", new { quizId = Question.QuizId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании вопроса");
                ModelState.AddModelError("", "Ошибка при сохранении вопроса");
                return Page();
            }
        }
    }
}