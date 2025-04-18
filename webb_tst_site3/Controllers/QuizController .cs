using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Security.Claims;

namespace webb_tst_site3.Controllers
{
    public class QuizController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuizController(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Quizzes.ToListAsync());
        }

        public async Task<IActionResult> Start(int id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == id);
            return View(quiz);
        }

        public async Task<IActionResult> Question(int quizId, int questionNumber)
        {
            var question = await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.QuizId == quizId)
                .OrderBy(q => q.Order)
                .Skip(questionNumber - 1)
                .FirstOrDefaultAsync();
            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessAnswer(int quizId, int questionId, int answerId, int questionNumber)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var answer = new UserQuizAnswer
            {
                UserId = userId,
                QuizId = quizId,
                QuestionId = questionId,
                AnswerId = answerId
            };

            _context.UserQuizAnswers.Add(answer);
            await _context.SaveChangesAsync();

            var nextQuestionNumber = questionNumber + 1;
            var totalQuestions = await _context.Questions.CountAsync(q => q.QuizId == quizId);

            if (nextQuestionNumber > totalQuestions)
            {
                return RedirectToAction("Result", new { quizId });
            }

            return RedirectToAction("Question", new { quizId, questionNumber = nextQuestionNumber });
        }

        public async Task<IActionResult> Result(int quizId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var userAnswers = await _context.UserQuizAnswers
                .Include(uqa => uqa.Answer)
                .Where(uqa => uqa.UserId == userId && uqa.QuizId == quizId)
                .ToListAsync();

            var resultScores = new Dictionary<int, int>();
            foreach (var answer in userAnswers)
            {
                var resultId = answer.Answer.ResultId;
                resultScores[resultId] = resultScores.GetValueOrDefault(resultId, 0) + answer.Answer.Score;
            }

            var maxScore = resultScores.Values.Max();
            var winningResultId = resultScores.FirstOrDefault(x => x.Value == maxScore).Key;

            var result = await _context.Results
                .FirstOrDefaultAsync(r => r.Id == winningResultId);

            return View(result);
        }
    }
}