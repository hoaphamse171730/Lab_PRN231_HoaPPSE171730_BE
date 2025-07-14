using IdentityAjaxClient.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Your existing EF + Identity setup
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not found");
builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(opts =>
    opts.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 1) Cookie authentication for MVC
builder.Services
  .AddAuthentication(options =>
  {
      options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  })
  .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
  {
      opts.LoginPath = "/Account/Login";
      opts.LogoutPath = "/Account/Logout";
      // you can tweak Cookie.SameSite, ExpireTimeSpan, etc.
  });
// 2) So we can inject IHttpContextAccessor if you need it
builder.Services.AddHttpContextAccessor();

// 3) HttpClientFactory for calling your API
builder.Services.AddHttpClient();

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseMigrationsEndPoint();
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
