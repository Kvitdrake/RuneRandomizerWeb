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

// �������� ��� ������
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddRazorPages(options =>
{
    // ������������ ������ ������ � ����� /Admin
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
    // ������ Login � Register ���������� ��� ����!
    options.Conventions.AllowAnonymousToPage("/Admin/Login");
    // ���� ���� �����������:
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
        options.Cookie.SameSite = SameSiteMode.Lax; // ��� Strict, �� Lax � ������ ����� ��� �������
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

app.UseAuthentication(); // �����: ������� ��������������
app.UseAuthorization();  // ����� �����������

app.MapRazorPages();
app.MapControllers();

app.Run();