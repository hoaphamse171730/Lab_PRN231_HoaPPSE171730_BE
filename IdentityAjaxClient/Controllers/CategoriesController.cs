using Microsoft.AspNetCore.Mvc;

namespace IdentityAjaxClient.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index() => View();
    }
}
