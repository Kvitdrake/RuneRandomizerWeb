using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using webb_tst_site3.Data;
using webb_tst_site3.Services;
using Microsoft.AspNetCore.Authentication.Cookies;



var builder = WebApplication.CreateBuilder(args);

// ��������� �������
builder.Services.AddRazorPages();

// ��������� ��
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 23))));

// ��������� �������������� (������ ��������)
/*builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options => {
    options.LoginPath = "/Admin/Login";
    options.AccessDeniedPath = "/Error";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

    // �����: ��������� �������������� ���������
   *//* options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context => {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
    };*//*
});*/
// � ������������ �������� (� ConfigureServices ��� builder.Services)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.AccessDeniedPath = "/Admin/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddAuthorization();

// ������������ �������
builder.Services.AddScoped<SettingsService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ������������ middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// �����: ������� middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();