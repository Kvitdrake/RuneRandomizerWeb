using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webb_tst_site3.Pages.Admin.Runes
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Rune> Runes { get; set; }

        public async Task OnGetAsync()
        {
            Runes = await _context.Runes
                .Include(r => r.SphereDescriptions)
                .ThenInclude(sd => sd.Sphere)
                .OrderBy(r => r.Order)
                .ToListAsync();
        }
    }
}