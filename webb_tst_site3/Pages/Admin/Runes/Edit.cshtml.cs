using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Runes
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Rune Rune { get; set; }

        public List<Sphere> AllSpheres { get; set; }
        public Dictionary<int, string> SphereDescriptions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Rune = await _context.Runes
                .Include(r => r.SphereDescriptions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (Rune == null)
            {
                return NotFound();
            }

            AllSpheres = await _context.Spheres.ToListAsync();

            // Заполняем описания для сфер
            foreach (var sphere in AllSpheres)
            {
                var description = Rune.SphereDescriptions?
                    .FirstOrDefault(sd => sd.SphereId == sphere.Id)?
                    .Description ?? "Нет описания";
                SphereDescriptions[sphere.Id] = description;
            }

            return Page();
        }

        public string GetSphereDescription(int sphereId)
        {
            return SphereDescriptions.TryGetValue(sphereId, out var description)
                ? description
                : "Нет описания";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AllSpheres = await _context.Spheres.ToListAsync();
                return Page();
            }

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
            existingRune.ImageUrl = Rune.ImageUrl ?? "/images/default-rune.png";
            existingRune.UpdatedAt = DateTime.UtcNow;

            // Обновляем описания для сфер
            foreach (var description in existingRune.SphereDescriptions)
            {
                if (Request.Form.TryGetValue($"SphereDescriptions[{description.SphereId}]", out var newDescription))
                {
                    description.Description = newDescription;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}