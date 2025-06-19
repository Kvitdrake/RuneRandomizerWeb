// Pages/Auth/Register.cshtml.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace webb_tst_site3.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public RegisterModel(AppDbContext context, IConfiguration configuration)
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
            [StringLength(50, ErrorMessage = "Имя пользователя должно быть от {2} до {1} символов", MinimumLength = 3)]
            public string Username { get; set; }

            [Required(ErrorMessage = "Email обязателен")]
            [EmailAddress(ErrorMessage = "Некорректный email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Пароль обязателен")]
            [StringLength(100, ErrorMessage = "Пароль должен быть от {2} до {1} символов", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            // Generate Telegram auth URL
            var botUsername = _configuration["TelegramBot:Username"];
            var authUrl = $"{Request.Scheme}://{Request.Host}/auth/telegram-callback?register=true";
            TelegramBotAuthUrl = $"https://t.me/{botUsername}?start={Uri.EscapeDataString(authUrl)}";
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var userExists = await _context.Users.AnyAsync(u => u.Username == Input.Username);
                if (userExists)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь с таким именем уже существует");
                    return Page();
                }

                var user = new User
                {
                    Username = Input.Username,
                    PasswordHash = HashPassword(Input.Password), // В реальном приложении используйте безопасное хеширование
                    Role = "User",
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await SignInUser(user, rememberMe: true);
                return LocalRedirect(ReturnUrl);
            }

            return Page();
        }

        private string HashPassword(string password)
        {
            // В реальном приложении используйте безопасное хеширование (например, BCrypt)
            return password + "_hash";
        }

        private async Task SignInUser(User user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
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
        }
    }
}