using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAjaxClient.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index() => View();
    }

}
