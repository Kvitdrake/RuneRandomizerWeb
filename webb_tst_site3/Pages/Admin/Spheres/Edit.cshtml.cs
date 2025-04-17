using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Spheres
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Sphere Sphere { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Sphere = await _context.Spheres.FindAsync(id);
            if (Sphere == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingSphere = await _context.Spheres.FindAsync(Sphere.Id);
            if (existingSphere == null)
            {
                return NotFound();
            }

            existingSphere.Name = Sphere.Name;
            existingSphere.Description = Sphere.Description;
            existingSphere.ImageUrl = Sphere.ImageUrl ?? "/images/default-sphere.png";
            existingSphere.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}