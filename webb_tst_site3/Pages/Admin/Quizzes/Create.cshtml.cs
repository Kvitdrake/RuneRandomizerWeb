// Pages/Admin/Quizzes/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;

namespace webb_tst_site3.Pages.Admin.Quizzes
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        [BindProperty]
        public Models.Quiz Quiz { get; set; }

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Quizzes.Add(Quiz);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Questions/Index", new { quizId = Quiz.Id });
        }
    }
}