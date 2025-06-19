using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.AspNetCore.Hosting;

namespace webb_tst_site3.Pages.Admin.Runes
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EditModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Rune Rune { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public string CurrentImageUrl { get; set; }
        public List<Sphere> AllSpheres { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Rune = await _context.Runes
                .Include(r => r.SphereDescriptions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Rune == null)
            {
                return NotFound();
            }

            CurrentImageUrl = Rune.ImageUrl;
            AllSpheres = await _context.Spheres.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            var existingRune = await _context.Runes
                .Include(r => r.SphereDescriptions)
                .FirstOrDefaultAsync(r => r.Id == Rune.Id);

            if (existingRune == null)
            {
                return NotFound();
            }

            // Обновляем основные свойства
            existingRune.Name = Rune.Name;
            existingRune.BaseDescription = Rune.BaseDescription;
            existingRune.UpdatedAt = DateTime.UtcNow;

            // Обработка изображения
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                // Удаляем старое изображение, если оно не дефолтное
                if (!string.IsNullOrEmpty(existingRune.ImageUrl) &&
                    !existingRune.ImageUrl.Contains("default-rune.png"))
                {
                    var oldFilePath = Path.Combine(_environment.WebRootPath, existingRune.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                existingRune.ImageUrl = "/uploads/" + uniqueFileName;
            }
            // Если изображение не загружено, оставляем текущее
            else if (!string.IsNullOrEmpty(Request.Form["Rune.ImageUrl"]))
            {
                existingRune.ImageUrl = Request.Form["Rune.ImageUrl"];
            }
            AllSpheres = await _context.Spheres.ToListAsync();

            // Обновляем описания по сферам
            foreach (var sphere in AllSpheres)
            {
                var sphereDesc = existingRune.SphereDescriptions
                    .FirstOrDefault(sd => sd.SphereId == sphere.Id);

                if (sphereDesc != null)
                {
                    if (Request.Form.TryGetValue($"SphereDescriptions[{sphere.Id}]", out var newDesc))
                    {
                        sphereDesc.Description = newDesc;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request.Form[$"SphereDescriptions[{sphere.Id}]"]))
                    {
                        existingRune.SphereDescriptions.Add(new RuneSphereDescription
                        {
                            SphereId = sphere.Id,
                            Description = Request.Form[$"SphereDescriptions[{sphere.Id}]"]
                        });
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Ошибка при сохранении: " + ex.Message);
                AllSpheres = await _context.Spheres.ToListAsync();
                return Page();
            }
        }
    }
}