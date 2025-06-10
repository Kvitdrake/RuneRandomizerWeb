using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Quiz
{
    public class QuestionModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionModel(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public Question Question { get; set; }
        public int CurrentQuestionNumber { get; set; }
        public int TotalQuestions { get; set; }
        public int ProgressPercentage => (int)((double)CurrentQuestionNumber / TotalQuestions * 100);

        public async Task<IActionResult> OnGetAsync(int quizId, int questionNumber)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null) return NotFound();

            TotalQuestions = quiz.Questions.Count;
            CurrentQuestionNumber = questionNumber;

            Question = await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.QuizId == quizId)
                .OrderBy(q => q.Order)
                .Skip(questionNumber - 1)
                .FirstOrDefaultAsync();

            if (Question == null)
            {
                return RedirectToPage("./Result", new { quizId });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int quizId, int questionId, int answerId, int currentQuestionNumber)
        {
            //var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            /*if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }*/

            var userAnswer = new UserQuizAnswer
            {
                //UserId = userId,
                QuizId = quizId,
                QuestionId = questionId,
                AnswerId = answerId
            };

            _context.UserQuizAnswers.Add(userAnswer);
            await _context.SaveChangesAsync();

            var nextQuestionNumber = currentQuestionNumber + 1;
            var totalQuestions = await _context.Questions
                .CountAsync(q => q.QuizId == quizId);

            if (nextQuestionNumber > totalQuestions)
            {
                return RedirectToPage("./Result", new { quizId });
            }

            return RedirectToPage("./Question", new { quizId, questionNumber = nextQuestionNumber });
        }
    }
}