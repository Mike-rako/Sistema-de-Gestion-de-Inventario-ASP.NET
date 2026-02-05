using ExamenMVC.Data;
using ExamenMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Connection string (usa "connection" y si no existe, "Default")
var cs = builder.Configuration.GetConnectionString("connection")
         ?? builder.Configuration.GetConnectionString("Default")
         ?? throw new InvalidOperationException("Falta ConnectionStrings: 'connection' o 'Default' en appsettings.json");

// EF Core + SQL Server
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));

// Auth por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Login";
        opt.AccessDeniedPath = "/Login";
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
        opt.SlidingExpiration = true;
    });

builder.Services.AddHttpContextAccessor();

// DI de servicios propios
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Productos}/{action=Index}/{id?}");

app.Run();
