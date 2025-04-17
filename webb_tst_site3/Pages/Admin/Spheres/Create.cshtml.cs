using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Spheres
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Sphere Sphere { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Установка значений по умолчанию
            Sphere.ImageUrl ??= "/images/default-sphere.png";
            Sphere.CreatedAt = DateTime.UtcNow;
            Sphere.UpdatedAt = DateTime.UtcNow;

            _context.Spheres.Add(Sphere);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}