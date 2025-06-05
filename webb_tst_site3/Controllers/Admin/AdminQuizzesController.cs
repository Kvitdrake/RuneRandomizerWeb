using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminQuizzesController : Controller
    {
        private readonly AppDbContext _context;

        public AdminQuizzesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Список всех квизов
        public async Task<IActionResult> Index()
        {
            return View(await _context.Quizzes.ToListAsync());
        }

        // GET: Форма создания
        public IActionResult Create()
        {
            return View();
        }

        // POST: Сохранение нового квиза
        [HttpPost]
        public async Task<IActionResult> Create(Quiz quiz)
        {
            _context.Add(quiz);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = quiz.Id });
        }

        // GET: Редактирование
        public async Task<IActionResult> Edit(int? id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .Include(q => q.Results)
                .FirstOrDefaultAsync(q => q.Id == id);
            return View(quiz);
        }

        // API: Обновление порядка вопросов
        [HttpPost]
        public async Task<IActionResult> UpdateQuestionOrder([FromBody] List<OrderItem> items)
        {
            foreach (var item in items)
            {
                var question = await _context.Questions.FindAsync(item.Id);
                question.Order = item.Order;
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        public class OrderItem { public int Id { get; set; } public int Order { get; set; } }
    }
}
