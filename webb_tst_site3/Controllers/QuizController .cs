using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Linq;
using System.Threading.Tasks;

namespace webb_tst_site3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuizController(AppDbContext context)
        {
            _context = context;
        }

        // Квизы
        [HttpGet("quizzes")]
        public async Task<IActionResult> GetAllQuizzes()
        {
            var quizzes = await _context.Quizzes.ToListAsync();
            return Ok(quizzes);
        }

        [HttpGet("quizzes/{id}")]
        public async Task<IActionResult> GetQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        [HttpPost("quizzes")]
        public async Task<IActionResult> CreateQuiz([FromBody] Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.Id }, quiz);
        }

        [HttpPut("quizzes/{id}")]
        public async Task<IActionResult> UpdateQuiz(int id, [FromBody] Quiz quiz)
        {
            if (id != quiz.Id) return BadRequest();
            _context.Entry(quiz).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("quizzes/{id}")]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null) return NotFound();
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Вопросы
        [HttpGet("quizzes/{quizId}/questions")]
        public async Task<IActionResult> GetQuestions(int quizId)
        {
            var questions = await _context.Questions
                .Where(q => q.QuizId == quizId)
                .ToListAsync();
            return Ok(questions);
        }

        [HttpPost("quizzes/{quizId}/questions")]
        public async Task<IActionResult> AddQuestion(int quizId, [FromBody] Question question)
        {
            question.QuizId = quizId;
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetQuestions), new { quizId }, question);
        }

        // Ответы
        [HttpGet("questions/{questionId}/answers")]
        public async Task<IActionResult> GetAnswers(int questionId)
        {
            var answers = await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .Include(a => a.Result)
                .ToListAsync();
            return Ok(answers);
        }

        [HttpPost("questions/{questionId}/answers")]
        public async Task<IActionResult> AddAnswer(int questionId, [FromBody] Answer answer)
        {
            answer.QuestionId = questionId;
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAnswers), new { questionId }, answer);
        }

        // Результаты
        [HttpGet("quizzes/{quizId}/results")]
        public async Task<IActionResult> GetResults(int quizId)
        {
            var results = await _context.Results
                .Where(r => r.QuizId == quizId)
                .ToListAsync();
            return Ok(results);
        }

        [HttpPost("quizzes/{quizId}/results")]
        public async Task<IActionResult> AddResult(int quizId, [FromBody] Result result)
        {
            result.QuizId = quizId;
            _context.Results.Add(result);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetResults), new { quizId }, result);
        }

        // Прохождение квиза
        [HttpPost("quizzes/{quizId}/submit")]
        public async Task<IActionResult> SubmitQuiz(int quizId, [FromBody] QuizSubmission submission)
        {
            // Сохраняем ответы пользователя
            foreach (var answer in submission.Answers)
            {
                _context.UserQuizAnswers.Add(new UserQuizAnswer
                {
                    UserId = submission.UserId,
                    QuizId = quizId,
                    QuestionId = answer.QuestionId,
                    AnswerId = answer.AnswerId
                });
            }

            await _context.SaveChangesAsync();

            // Определяем результат
            var result = await CalculateResult(quizId, submission.UserId);
            return Ok(result);
        }

        private async Task<Result> CalculateResult(int quizId, string userId)
        {
            var userAnswers = await _context.UserQuizAnswers
                .Include(uqa => uqa.Answer)
                .Where(uqa => uqa.QuizId == quizId && uqa.UserId == userId)
                .ToListAsync();

            var resultScores = userAnswers
                .GroupBy(uqa => uqa.Answer.ResultId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Answer.Score));

            var winningResultId = resultScores.OrderByDescending(x => x.Value).First().Key;
            return await _context.Results.FindAsync(winningResultId);
        }
    }

    public class QuizSubmission
    {
        public string UserId { get; set; }
        public List<QuestionAnswer> Answers { get; set; }
    }

    public class QuestionAnswer
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
}