using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IdentityAjaxClient.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IConfiguration _config;

        public OrdersController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult History() => View();

        [HttpGet]
        public IActionResult Details(int id) => View(id);


        public IActionResult Cart() => View();
        [HttpGet("orders/vnpay-return")]
        [AllowAnonymous]
        public IActionResult VnPayReturn()
        {
            // 1) Grab all the VNPAY query parameters
            var query = HttpContext.Request.Query
                        .Where(kvp => kvp.Key.StartsWith("vnp_"))
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            // 2) Extract and remove secure hash from query
            if (!query.TryGetValue("vnp_SecureHash", out var receivedHash))
                return View("Failed");

            query.Remove("vnp_SecureHash");
            query.Remove("vnp_SecureHashType");

            // 3) Recompute hash
            var secret = _config["VnPay:HashSecret"]!;
            var sorted = query.OrderBy(k => k.Key)
                              .Select(k => $"{k.Key}={HttpUtility.UrlEncode(k.Value)}")
                              .ToArray();
            var hashData = string.Join("&", sorted);
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secret));
            var computedHash = hmac
                .ComputeHash(Encoding.UTF8.GetBytes(hashData))
                .Aggregate(new StringBuilder(), (sb, b) => sb.Append(b.ToString("X2")))
                .ToString();

            if (!computedHash.Equals(receivedHash, StringComparison.OrdinalIgnoreCase))
            {
                // tampered
                return View("Failed");
            }

            // 4) Check VNPAY response code
            query.TryGetValue("vnp_ResponseCode", out var code);
            if (code == "00")
            {
                return View("Success");
            }

            return View("Failed");
        }

    }
}
