// Pages/Auth/Login.cshtml.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace webb_tst_site3.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public LoginModel(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public bool ShowTelegramButton { get; set; } = true;
        public string TelegramBotAuthUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Имя пользователя обязательно")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Пароль обязателен")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Запомнить меня?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Generate Telegram auth URL
            var botUsername = _configuration["TelegramBot:Username"];
            var authUrl = $"{Request.Scheme}://{Request.Host}/auth/telegram-callback";
            TelegramBotAuthUrl = $"https://t.me/{botUsername}?start={Uri.EscapeDataString(authUrl)}";
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == Input.Username);

                if (user == null || !VerifyPassword(webb_tst_site3.Extensions.StringExtensions.GetSHA256Hash(Input.Password), user.PasswordHash))
                {
                    ModelState.AddModelError(string.Empty, "Неверное имя пользователя или пароль");
                    return Page();
                }

                await SignInUser(user, Input.RememberMe);
                return LocalRedirect(ReturnUrl);
            }

            return Page();
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            // Реализуйте проверку пароля (например, с помощью BCrypt)
            // Это упрощённый пример - в реальном приложении используйте безопасное хеширование
            return password == storedHash;
        }

        private async Task SignInUser(User user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? user.TelegramUsername ?? "User"),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Обновляем время последнего входа
            user.LastLoginAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}