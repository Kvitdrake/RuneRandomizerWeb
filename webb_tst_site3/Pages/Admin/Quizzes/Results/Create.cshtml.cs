using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace webb_tst_site3.Pages.Admin.Quizzes.Results
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CreateModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Result Result { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public Models.Quiz Quiz { get; set; }

        public async Task<IActionResult> OnGetAsync(int quizId)
        {
            Quiz = await _context.Quizzes.FindAsync(quizId);
            if (Quiz == null)
            {
                return NotFound();
            }

            Result = new Result { QuizId = quizId };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Quiz = await _context.Quizzes.FindAsync(Result.QuizId);
                return Page();
            }

            // Обработка изображения
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images/results");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                Result.ImageUrl = "/images/results/" + uniqueFileName;
            }

            _context.Results.Add(Result);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index", new { quizId = Result.QuizId });
        }
    }
}