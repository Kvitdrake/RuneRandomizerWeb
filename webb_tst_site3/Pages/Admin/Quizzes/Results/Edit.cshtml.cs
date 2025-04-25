// Pages/Admin/Quizzes/Results/Edit.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace webb_tst_site3.Pages.Admin.Quizzes.Results
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public Result Result { get; set; }

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        [BindProperty]
        public bool DeleteImage { get; set; }

        public Models.Quiz Quiz { get; set; }

        public EditModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Result = await _context.Results
                .Include(r => r.Quiz)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Result == null) return NotFound();

            Quiz = Result.Quiz;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Удаляем ошибки валидации для Quiz
            ModelState.Remove("Result.Quiz");

            if (string.IsNullOrWhiteSpace(Result.Name))
                ModelState.AddModelError("Result.Name", "Название обязательно");

            if (string.IsNullOrWhiteSpace(Result.Description))
                ModelState.AddModelError("Result.Description", "Описание обязательно");

            if (!ModelState.IsValid)
            {
                Quiz = await _context.Quizzes.FindAsync(Result.QuizId);
                return Page();
            }

            // Обработка изображения
            if (DeleteImage && !string.IsNullOrEmpty(Result.ImageUrl))
            {
                var filePath = Path.Combine(_environment.WebRootPath, Result.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                Result.ImageUrl = null;
            }

            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Удаляем старое изображение
                if (!string.IsNullOrEmpty(Result.ImageUrl))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, Result.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Сохраняем новое изображение
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

            _context.Attach(Result).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResultExists(Result.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToPage("./Index", new { quizId = Result.QuizId });
        }

        private bool ResultExists(int id)
        {
            return _context.Results.Any(e => e.Id == id);
        }
    }
}