using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace webb_tst_site3.Pages.Admin.Quizzes.Results
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DeleteModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Result Result { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Result = await _context.Results.FindAsync(id);

            if (Result == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Result = await _context.Results.FindAsync(id);

            if (Result != null)
            {
                // Удаляем изображение
                if (!string.IsNullOrEmpty(Result.ImageUrl))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, Result.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Results.Remove(Result);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index", new { quizId = Result.QuizId });
        }
    }
}