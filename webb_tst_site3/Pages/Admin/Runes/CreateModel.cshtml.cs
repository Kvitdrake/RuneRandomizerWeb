using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace webb_tst_site3.Pages.Admin.Runes
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public CreateModel(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [BindProperty]
        public Rune Rune { get; set; } = new Rune();

        [BindProperty]
        public Dictionary<int, string> SphereDescriptions { get; set; } = new();

        public List<Sphere> AllSpheres { get; set; }

        public async Task OnGetAsync()
        {
            AllSpheres = await _context.Spheres.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            AllSpheres = await _context.Spheres.ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Rune.ImageFile != null && Rune.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/runes");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Rune.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Rune.ImageFile.CopyToAsync(stream);
                }

                Rune.ImageUrl = "/uploads/runes/" + fileName;
            }
            else
            {
                Rune.ImageUrl ??= "/images/default-rune.png";
            }

            Rune.CreatedAt = DateTime.UtcNow;
            Rune.UpdatedAt = DateTime.UtcNow;

            _context.Runes.Add(Rune);
            await _context.SaveChangesAsync();

            foreach (var sphere in AllSpheres)
            {
                var desc = Request.Form[$"SphereDescriptions[{sphere.Id}]"];
                _context.RuneSphereDescriptions.Add(new RuneSphereDescription
                {
                    RuneId = Rune.Id,
                    SphereId = sphere.Id,
                    Description = desc
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}