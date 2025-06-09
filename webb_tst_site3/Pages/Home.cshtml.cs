using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Models;
using webb_tst_site3.Data;

namespace webb_tst_site3.Pages
{
    public class HomeModel : PageModel
    {
        private readonly SettingsService _settingsService;

        public HomeModel(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private readonly AppDbContext _context;
        public HomeModel(AppDbContext context) { _context = context; }
        public List<Article> Articles { get; set; }
        public string MainContent { get; set; }
        public bool ShowRandomizer { get; set; } = true;
        public bool ShowQuizzes { get; set; } = true;

        public async Task OnGetAsync()
        {
            MainContent = await _settingsService.GetSettingAsync("HomePageContent");
            ShowRandomizer = (await _settingsService.GetSettingAsync("ShowRandomizer", "true")) == "true";
            ShowQuizzes = (await _settingsService.GetSettingAsync("ShowQuizzes", "true")) == "true";
            Articles = await _context.Articles.OrderByDescending(a => a.CreatedAt).ToListAsync();

        }
    }
}