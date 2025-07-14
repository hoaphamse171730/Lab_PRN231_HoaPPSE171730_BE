using BusinessObjects.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAjaxClient.Controllers
{
    public class OrchidsController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Browse() => View();

    }
}
