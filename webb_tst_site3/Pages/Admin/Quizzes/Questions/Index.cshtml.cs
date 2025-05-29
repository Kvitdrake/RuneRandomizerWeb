using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int QuizId { get; set; }

        public Models.Quiz Quiz { get; set; }
        public IList<Question> Questions { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Загружаем квиз с вопросами
            Quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == QuizId);

            if (Quiz == null)
            {
                return NotFound();
            }

            Questions = Quiz.Questions.OrderBy(q => q.Order).ToList();
            return Page();
        }
    }
}