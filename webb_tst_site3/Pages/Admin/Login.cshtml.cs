using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using System.Linq;

namespace webb_tst_site3.Pages.Admin
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Заполните все поля";
                return Page();
            }

            // Найти пользователя по имени
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == Username);

            if (user == null || !VerifyPassword(webb_tst_site3.Extensions.StringExtensions.GetSHA256Hash(Password), user.PasswordHash) || user.Role != "Admin")
            {
                ErrorMessage = "Неверный логин/пароль или нет прав администратора";
                return Page();
            }

            // Аутентификация через куки
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Admin/Index");
        }

        // Пример хэширования (замените на свой)
        private bool VerifyPassword(string password, string passwordHash)
        {
            // Для теста: храните пароль в открытом виде!
            // В проде: используйте BCrypt/SHA256 и т.п.
            return password == passwordHash;
        }
    }
}