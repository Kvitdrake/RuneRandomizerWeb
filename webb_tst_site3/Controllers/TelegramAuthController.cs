using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace webb_tst_site3.Controllers
{
    [Route("auth")]
    public class TelegramAuthController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public TelegramAuthController(AppDbContext context, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _context = context;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        [HttpGet("telegram-callback")]
        public async Task<IActionResult> TelegramCallback([FromQuery] string register, [FromQuery] string data)
        {
            // В реальном приложении здесь должна быть проверка хеша от Telegram
            // Для упрощения мы будем использовать временный токен из кеша

            if (string.IsNullOrEmpty(data))
            {
                return RedirectToPage("/Auth/Login", new { error = "invalid_token" });
            }

            // Получаем данные из кеша
            if (!_memoryCache.TryGetValue($"telegram_auth_{data}", out TelegramUserData telegramData))
            {
                return RedirectToPage("/Auth/Login", new { error = "token_expired" });
            }

            // Ищем пользователя по Telegram ID
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.TelegramId == telegramData.Id);

            if (user == null && register != "true")
            {
                // Пользователь не найден и не пытается зарегистрироваться
                return RedirectToPage("/Auth/Login", new { error = "user_not_found" });
            }

            if (user == null)
            {
                // Регистрация нового пользователя
                user = new User
                {
                    TelegramId = telegramData.Id,
                    TelegramUsername = telegramData.Username,
                    FirstName = telegramData.FirstName,
                    LastName = telegramData.LastName,
                    PhotoUrl = telegramData.PhotoUrl,
                    Role = "User",
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Обновляем данные существующего пользователя
                user.TelegramUsername = telegramData.Username;
                user.FirstName = telegramData.FirstName;
                user.LastName = telegramData.LastName;
                user.PhotoUrl = telegramData.PhotoUrl;
                user.LastLoginAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            // Аутентифицируем пользователя
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.TelegramUsername ?? $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("TelegramId", user.TelegramId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToPage("/Home");
        }

        [HttpGet("telegram-connect")]
        public async Task<IActionResult> TelegramConnect([FromQuery] string data)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Auth/Login");
            }

            if (string.IsNullOrEmpty(data))
            {
                return RedirectToPage("/Profile", new { error = "invalid_token" });
            }

            if (!_memoryCache.TryGetValue($"telegram_auth_{data}", out TelegramUserData telegramData))
            {
                return RedirectToPage("/Profile", new { error = "token_expired" });
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return RedirectToPage("/Profile", new { error = "user_not_found" });
            }

            // Привязываем Telegram к существующему аккаунту
            user.TelegramId = telegramData.Id;
            user.TelegramUsername = telegramData.Username;
            user.FirstName = telegramData.FirstName;
            user.LastName = telegramData.LastName;
            user.PhotoUrl = telegramData.PhotoUrl;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Profile", new { success = "telegram_connected" });
        }
    }

    public class TelegramUserData
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhotoUrl { get; set; }
    }
}