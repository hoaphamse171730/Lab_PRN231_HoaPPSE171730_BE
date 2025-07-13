using IdentityAjaxClient.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. EF Core + Identity DbContext (your existing ApplicationDbContext)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. ASP.NET Core Identity
builder.Services
    .AddDefaultIdentity<IdentityUser>(opts => opts.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Cookie Authentication (for MVC User.Identity)
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

// 4. Register IHttpClientFactory so your AccountController can inject it
builder.Services.AddHttpClient();

// 5. MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// pipeline (keep your existing dev/prod checks)
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// this must come *after* UseRouting and *before* Map*
app.UseAuthentication();
app.UseAuthorization();

// your default routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapRazorPages();

app.Run();
