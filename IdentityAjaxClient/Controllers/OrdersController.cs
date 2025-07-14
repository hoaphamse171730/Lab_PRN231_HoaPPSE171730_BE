using Microsoft.AspNetCore.Mvc;

namespace IdentityAjaxClient.Controllers
{
    public class OrdersController : Controller
    {
        [HttpGet]
        public IActionResult History() => View();

        [HttpGet]
        public IActionResult Details(int id) => View(id);


        public IActionResult Cart() => View();
    }
}
