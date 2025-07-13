using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAjaxClient.Controllers
{
    [Authorize]
    public class OrchidsController : Controller
    {
        public IActionResult Index() => View();
    }
}
