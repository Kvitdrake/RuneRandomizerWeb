using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webb_tst_site3.Pages.Admin.Quizzes.Questions.Answers
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        [FromRoute]
        public int QuizId { get; set; }

        [FromRoute]
        public int? QuestionId { get; set; }

        public Models. Quiz Quiz { get; set; }
        public Question Question { get; set; }
        public List<Answer> Answers { get; set; }
        public async Task OnGetAsync(int questionId)
        {
            Question = await _context.Questions
                .Include(q => q.Quiz)
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (Question == null)
            {
                return;
            }

            Quiz = Question.Quiz;
            Answers = await _context.Answers
                .Include(a => a.Result)
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }
    }
}