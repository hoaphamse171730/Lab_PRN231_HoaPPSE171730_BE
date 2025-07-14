using Microsoft.AspNetCore.Mvc;

namespace IdentityAjaxClient.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Cart() => View();
    }
}
