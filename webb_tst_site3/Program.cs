using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
builder.Services.AddRazorPages();

// Настройка БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 23))));

// Настройка аутентификации через куки
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Error";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAntiforgery();

var app = builder.Build();

/*// Инициализация БД
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Создаем администратора если его нет
    if (!db.Users.Any(u => u.Username == "admin"))
    {
        db.Users.Add(new User
        {
            Username = "admin",
            PasswordHash = HashPassword("admin123"),
            Role = "Admin"
        });
        await db.SaveChangesAsync();
    }
}*/

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();

// Хелпер для хеширования пароля
string HashPassword(string password)
{
    using var sha256 = SHA256.Create();
    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
}