using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Runes
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
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
            if (!ModelState.IsValid)
            {
                AllSpheres = await _context.Spheres.ToListAsync();
                return Page();
            }

            // Установка значений по умолчанию
            Rune.ImageUrl ??= "/images/default-rune.png";
            Rune.CreatedAt = DateTime.UtcNow;
            Rune.UpdatedAt = DateTime.UtcNow;

            _context.Runes.Add(Rune);
            await _context.SaveChangesAsync();

            // Добавление связей со сферами
            foreach (var sphere in await _context.Spheres.ToListAsync())
            {
                _context.RuneSphereDescriptions.Add(new RuneSphereDescription
                {
                    RuneId = Rune.Id,
                    SphereId = sphere.Id,
                    Description = "Нет описания" // или можно оставить пустым
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}