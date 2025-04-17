using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;

namespace webb_tst_site3.Pages.Admin.Spheres
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Sphere> Spheres { get; set; }

        public async Task OnGetAsync()
        {
            Spheres = await _context.Spheres
                .OrderBy(s => s.Name)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var sphere = await _context.Spheres.FindAsync(id);
            if (sphere != null)
            {
                _context.Spheres.Remove(sphere);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}