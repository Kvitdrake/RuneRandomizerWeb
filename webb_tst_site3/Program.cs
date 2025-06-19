using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using webb_tst_site3.Data;
using webb_tst_site3.Models;
using webb_tst_site3.Services;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Аутентификация
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

// Кеш для временных токенов
builder.Services.AddMemoryCache();

// Контроллеры и Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Telegram Bot Service
builder.Services.AddHostedService<TelegramBotService>();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Обработчик для Telegram callback
app.MapGet("/auth/telegram-callback", async (
    [FromQuery] string token,
    [FromServices] IMemoryCache cache,
    [FromServices] AppDbContext db,
    HttpContext context) =>
{
    if (!cache.TryGetValue($"telegram_auth_{token}", out long telegramId))
    {
        context.Response.Redirect("/auth/login?error=invalid_token");
        return;
    }

    cache.Remove($"telegram_auth_{token}");

    var user = await db.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
    if (user == null)
    {
        user = new User
        {
            TelegramId = telegramId,
            Username = $"tg_{telegramId}",
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Role, user.Role),
        new("TelegramId", telegramId.ToString())
    };

    await context.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(new ClaimsIdentity(claims)),
        new AuthenticationProperties { IsPersistent = true });

    context.Response.Redirect("/");
});

app.MapRazorPages();
app.MapControllers();

app.Run();