using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Hosting;
using System.IO;

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

        public List<Sphere> AllSpheres { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Rune = await _context.Runes
                .Include(r => r.SphereDescriptions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Rune == null)
                return NotFound();

            AllSpheres = await _context.Spheres.ToListAsync();
            return Page();
        }

        public string GetSphereDescription(int sphereId)
        {
            return Rune?.SphereDescriptions?.FirstOrDefault(sd => sd.SphereId == sphereId)?.Description ?? "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AllSpheres = await _context.Spheres.ToListAsync();

            // Найти существующую руну в базе с описаниями по сферам
            var existingRune = await _context.Runes
                .Include(r => r.SphereDescriptions)
                .FirstOrDefaultAsync(r => r.Id == Rune.Id);

            if (existingRune == null)
                return NotFound();

            // Обновляем основные свойства
            existingRune.Name = Rune.Name;
            existingRune.BaseDescription = Rune.BaseDescription;
            existingRune.UpdatedAt = DateTime.UtcNow;

            // Обработка загрузки изображения
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

                existingRune.ImageUrl = "/uploads/" + uniqueFileName;
            }
            else if (string.IsNullOrWhiteSpace(Rune.ImageUrl))
            {
                existingRune.ImageUrl = "/images/default-rune.png";
            }

            // Обновляем описания по сферам ИЗ Request.Form
            foreach (var sd in existingRune.SphereDescriptions)
            {
                if (Request.Form.TryGetValue($"SphereDescriptions[{sd.SphereId}]", out var newDesc))
                {
                    sd.Description = newDesc;
                }
            }

            await _context.SaveChangesAsync();

            // После успешного сохранения возвращаемся к списку рун
            return RedirectToPage("./Index");
        }
    }
}