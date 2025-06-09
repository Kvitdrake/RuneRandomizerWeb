using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace webb_tst_site3.Pages.Admin.Runes
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Rune> Runes { get; private set; } = new List<Rune>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                Runes = await _context.Runes
                    .AsNoTracking()
                    .Include(r => r.SphereDescriptions)
                        .ThenInclude(sd => sd.Sphere)
                    .OrderBy(r => r.Order)
                    .ToListAsync() ?? new List<Rune>();

                return Page();
            }
            catch (System.Exception ex)
            {
                // Логирование ошибки
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var rune = await _context.Runes.FindAsync(id);
                if (rune == null)
                {
                    return NotFound();
                }

                _context.Runes.Remove(rune);
                await _context.SaveChangesAsync();

                return RedirectToPage();
            }
            catch (System.Exception ex)
            {
                // Логирование ошибки
                return RedirectToPage("/Error");
            }
        }
    }
}