using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace webb_tst_site3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public string TelegramBotAuthUrl { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToPage("/Admin/Index");
                }
                else
                {
                    return RedirectToPage("/Home");
                }
            }

            // Generate Telegram auth URL
            var botUsername = _configuration["TelegramBot:Username"];
            var authUrl = $"{Request.Scheme}://{Request.Host}/auth/telegram-callback";
            TelegramBotAuthUrl = $"https://t.me/{botUsername}?start={Uri.EscapeDataString(authUrl)}";

            return Page();
        }
    }
}