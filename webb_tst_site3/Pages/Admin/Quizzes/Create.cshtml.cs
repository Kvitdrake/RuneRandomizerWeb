using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace webb_tst_site3.Pages.Admin.Quizzes
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
        public Models.Quiz Quiz { get; set; }

        public void OnGet()
        {
            _logger.LogInformation("Открыта страница создания квиза");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Quizzes.Add(Quiz);
            await _context.SaveChangesAsync();

            // Перенаправляем на страницу управления вопросами
            return RedirectToPage("./Questions/Index", new { quizId = Quiz.Id });
        }
    }
}