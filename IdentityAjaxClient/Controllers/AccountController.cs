using BusinessObjects.Models.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAjaxClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpFactory;

        public AccountController(IHttpClientFactory httpFactory)
        {
            _httpFactory = httpFactory;
        }
        [HttpGet]
        public IActionResult Register() => View(new RegisterRequest());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpFactory.CreateClient();
            var resp = await client.PostAsJsonAsync(
                "https://localhost:7244/api/auth/register", model);

            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View(model);
            }

            // On success, redirect to Login
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var client = _httpFactory.CreateClient();
            var loginReq = new LoginRequest { Email = email, Password = password };

            var response = await client.PostAsJsonAsync(
                "https://localhost:7244/api/auth/login",
                loginReq);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Login failed");
                return View();
            }

            var loginRes = await response.Content
                                         .ReadFromJsonAsync<LoginResponse>();

            if (loginRes == null)
            {
                ModelState.AddModelError("", "Invalid response");
                return View();
            }

            // Store token in a secure, HTTP-only cookie
            Response.Cookies.Append("AuthToken",
                loginRes.Token,
                new CookieOptions { HttpOnly = true, Secure = true });

            return RedirectToAction("Index", "Orchids");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login");
        }


    }
}
