// Pages/Admin/Quizzes/Results/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;

namespace webb_tst_site3.Pages.Admin.Quizzes.Results
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        [BindProperty]
        public Result Result { get; set; } = new();

        public Models.Quiz Quiz { get; set; }

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int quizId)
        {
            Quiz = await _context.Quizzes.FindAsync(quizId);
            if (Quiz == null) return NotFound();

            Result.QuizId = quizId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int quizId)
        {
            Quiz = await _context.Quizzes.FindAsync(quizId);
            if (Quiz == null) return NotFound();

            Result.QuizId = quizId;

            // Удаляем ошибки валидации для Quiz, если они есть
            ModelState.Remove("Result.Quiz");

            if (string.IsNullOrWhiteSpace(Result.Name))
                ModelState.AddModelError("Result.Name", "Название обязательно");

            if (string.IsNullOrWhiteSpace(Result.Description))
                ModelState.AddModelError("Result.Description", "Описание обязательно");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            Result.ImageUrl = string.IsNullOrWhiteSpace(Result.ImageUrl) ? null : Result.ImageUrl;

            _context.Results.Add(Result);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { quizId });
        }
    }
}