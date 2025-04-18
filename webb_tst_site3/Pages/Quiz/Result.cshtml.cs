using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Quiz
{
    public class ResultModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ResultModel(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public Result Result { get; set; }

        public async Task<IActionResult> OnGetAsync(int quizId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Получаем все ответы пользователя для этого теста
            var userAnswers = await _context.UserQuizAnswers
                .Include(uqa => uqa.Answer)
                .Where(uqa => uqa.UserId == userId && uqa.QuizId == quizId)
                .ToListAsync();

            // Подсчитываем баллы для каждого возможного результата
            var resultScores = userAnswers
                .GroupBy(uqa => uqa.Answer.ResultId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Answer.Score));

            // Находим результат с максимальным количеством баллов
            if (resultScores.Any())
            {
                var maxScore = resultScores.Values.Max();
                var winningResultId = resultScores.FirstOrDefault(x => x.Value == maxScore).Key;

                Result = await _context.Results
                    .FirstOrDefaultAsync(r => r.Id == winningResultId);
            }

            if (Result == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}