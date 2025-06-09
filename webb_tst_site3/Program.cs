using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Политика для админа
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddRazorPages(options =>
{
    // Ограничиваем доступ только к папке /Admin
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
    // ДЕЛАЕМ Login и Register ДОСТУПНЫМИ для всех!
    options.Conventions.AllowAnonymousToPage("/Admin/Login");
    // Если есть регистрация:
    // options.Conventions.AllowAnonymousToPage("/Admin/Register");
});
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<webb_tst_site3.Services.SettingsService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax; // или Strict, но Lax — обычно проще для локалки
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.Name = "RuneRandomizerAuth";
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Важно: сначала аутентификация
app.UseAuthorization();  // потом авторизация

app.MapRazorPages();
app.MapControllers();

app.Run();