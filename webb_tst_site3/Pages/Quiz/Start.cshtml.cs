using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace webb_tst_site3.Pages.Quiz
{
    public class StartModel : PageModel
    {
        private readonly AppDbContext _context;

        public StartModel(AppDbContext context)
        {
            _context = context;
        }

        public Models.Quiz Quiz { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Quiz = await _context.Quizzes.FindAsync(id);

            if (Quiz == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}