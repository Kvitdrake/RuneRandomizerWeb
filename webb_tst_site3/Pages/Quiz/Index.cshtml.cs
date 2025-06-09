using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace webb_tst_site3.Pages.Quiz
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Models.Quiz> Quizzes { get; set; }

        public async Task OnGetAsync()
        {
            Quizzes = await _context.Quizzes
                .Where(q => q.IsPublished)
                .OrderBy(q => q.Title)
                .ToListAsync();
        }
    }
}