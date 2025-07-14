using BusinessObjects.Models.Accounts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityAjaxClient.Controllers
{

    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login() => View();

        // GET /Account/Register
        [HttpGet]
        public IActionResult Register() => View(new RegisterRequest());

        // POST /Account/Register (traditional server-side)
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = HttpContext.RequestServices
                            .GetRequiredService<IHttpClientFactory>()
                            .CreateClient();
            var resp = await client.PostAsJsonAsync(
                BusinessObjects.Shared.ApiRoutes.AuthRegister, model);

            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                ModelState.AddModelError("", error);
                return View(model);
            }

            return RedirectToAction("Login");
        }

    }
}
