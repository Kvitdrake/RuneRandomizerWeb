using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Models;
using webb_tst_site3.Data;
using System.Linq;

namespace webb_tst_site3.Pages.Admin.Settings
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        public IndexModel(AppDbContext context) => _context = context;

        [BindProperty]
        public SiteSetting Settings { get; set; } = new SiteSetting();

        public void OnGet()
        {
            Settings = _context.SiteSettings.FirstOrDefault() ?? new SiteSetting();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var existing = _context.SiteSettings.FirstOrDefault();
            if (existing == null)
            {
                _context.SiteSettings.Add(Settings);
            }
            else
            {
                existing.SiteName = Settings.SiteName;
                existing.MainPageText = Settings.MainPageText;
                // Дополнительные поля тоже обновляйте здесь
            }
            _context.SaveChanges();
            TempData["Success"] = "Настройки сохранены!";
            return RedirectToPage();
        }
    }
}