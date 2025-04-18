using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public int RunesCount { get; set; }
        public int SpheresCount { get; set; }
        public int QuizzesCount { get; set; }

        public async Task OnGetAsync()
        {
            RunesCount = await _context.Runes.CountAsync();
            SpheresCount = await _context.Spheres.CountAsync();
            QuizzesCount = await _context.Quizzes.CountAsync();
        }
    }
}