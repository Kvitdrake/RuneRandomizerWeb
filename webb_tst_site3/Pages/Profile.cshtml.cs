using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.Extensions.Configuration;

namespace webb_tst_site3.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ProfileModel(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? TelegramUsername { get; set; }
        public string? UserPhotoUrl { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLoginAt { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsTelegramUser { get; set; }
        public bool ShowTelegramConnect { get; set; }
        public string? TelegramBotConnectUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Auth/Login");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null)
            {
                return NotFound();
            }

            Username = user.Username ?? user.TelegramUsername ?? "Пользователь";
            FirstName = user.FirstName;
            LastName = user.LastName;
            TelegramUsername = user.TelegramUsername;
            UserPhotoUrl = user.PhotoUrl;
            Role = user.Role;
            CreatedAt = user.CreatedAt;
            LastLoginAt = user.LastLoginAt;
            IsAdmin = user.Role == "Admin";
            IsTelegramUser = user.TelegramId.HasValue;

            // Показывать кнопку привязки Telegram только для обычных пользователей
            ShowTelegramConnect = !IsTelegramUser && _configuration.GetSection("TelegramBot").Exists();

            if (ShowTelegramConnect)
            {
                var botUsername = _configuration["TelegramBot:Username"];
                var authUrl = $"{Request.Scheme}://{Request.Host}/auth/telegram-connect";
                TelegramBotConnectUrl = $"https://t.me/{botUsername}?start={Uri.EscapeDataString(authUrl)}";
            }

            return Page();
        }
    }
}