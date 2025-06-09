using Microsoft.AspNetCore.Mvc.RazorPages;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webb_tst_site3.Services;

namespace webb_tst_site3.Pages
{
    public class HomeModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly SettingsService _settingsService;

        public HomeModel(AppDbContext context, SettingsService settingsService)
        {
            _context = context;
            _settingsService = settingsService;
        }

        public List<Article> Articles { get; set; }
        public string MainContent { get; set; }
        public bool ShowRandomizer { get; set; }
        public bool ShowQuizzes { get; set; }
        public string CustomBlock { get; set; }

        public async Task OnGetAsync()
        {
            MainContent = await _settingsService.GetSettingAsync("HomePageContent");
            ShowRandomizer = (await _settingsService.GetSettingAsync("ShowRandomizer", "true")) == "true";
            ShowQuizzes = (await _settingsService.GetSettingAsync("ShowQuizzes", "true")) == "true";
            CustomBlock = await _settingsService.GetSettingAsync("CustomBlock");
            Articles = await _context.Articles
                .Where(a => a.IsPublished)
                .OrderByDescending(a => a.CreatedAt)
                .Take(8)
                .ToListAsync();
        }
    }
}