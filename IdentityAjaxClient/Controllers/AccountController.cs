using BusinessObjects.Models.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityAjaxClient.Controllers
{

    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;
        public AccountController(IHttpClientFactory httpFactory)
            => _httpFactory = httpFactory;

        [HttpGet]
        public IActionResult Login() => View(new LoginRequest());

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpFactory.CreateClient();
            var resp = await client.PostAsJsonAsync(
                BusinessObjects.Shared.ApiRoutes.AuthLogin, model);

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            var result = await resp.Content
                                   .ReadFromJsonAsync<LoginResponse>();
            if (result is null)
            {
                ModelState.AddModelError("", "Invalid response from server");
                return View(model);
            }

            // Store the JWT in a cookie (optional, for your AJAX calls)
            Response.Cookies.Append("AuthToken",
                result.Token,
                new CookieOptions { HttpOnly = true, Secure = true });

            // **Sign in the MVC cookie** **
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,    model.Email),
                new Claim("access_token",     result.Token)
            };
            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            // Redirect to Home (navbar will now see you as authenticated)
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Remove the JWT cookie too
            Response.Cookies.Delete("AuthToken");
            // Sign out the MVC cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
