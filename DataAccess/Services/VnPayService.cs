using BusinessObjects.Models.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace DataAccess.Services
{
    public class VnPayService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpAccessor;
        // temporarily hold the in‑flight carts by txnRef
        private static readonly ConcurrentDictionary<string, PlaceOrderRequest> _pending
            = new();

        public VnPayService(IConfiguration config, IHttpContextAccessor httpAccessor)
        {
            _config = config;
            _httpAccessor = httpAccessor;
        }

        public string CreatePaymentUrl(decimal amount, PlaceOrderRequest req)
        {
            // 1) Generate a txnRef and stash the request
            var txnRef = Guid.NewGuid().ToString("N");
            _pending[txnRef] = req;

            // 2) Load your VNPay config
            var vnpUrl = _config["VnPay:Url"]!;
            var tmnCode = _config["VnPay:TmnCode"]!;
            var hashSecret = _config["VnPay:HashSecret"]!;
            var returnUrl = _config["VnPay:ReturnUrl"]!;

            // 3) Grab client IP (sandbox on localhost will be ::1)
            var ip = _httpAccessor.HttpContext?
                        .Connection?
                        .RemoteIpAddress?
                        .ToString() ?? "127.0.0.1";

            // 4) Build the *raw* parameter bag (no SecureHash entries yet)
            var vnpParams = new Dictionary<string, string>
            {
                ["vnp_Version"] = "2.1.0",
                ["vnp_Command"] = "pay",
                ["vnp_TmnCode"] = tmnCode,
                ["vnp_Amount"] = ((int)(amount * 100)).ToString(),
                ["vnp_CurrCode"] = "VND",
                ["vnp_TxnRef"] = txnRef,
                ["vnp_OrderInfo"] = $"Thanh toán {req.Items.Count} sản phẩm",
                ["vnp_OrderType"] = "other",
                ["vnp_Locale"] = "vn",
                ["vnp_ReturnUrl"] = returnUrl,
                ["vnp_IpAddr"] = ip,
                ["vnp_CreateDate"] = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
            };

            // 5) Sort by key, then percent‑encode both key and value, and join with &
            var ordered = vnpParams
                .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                .Select(kv =>
                    $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}");

            var dataToHash = string.Join("&", ordered);

            // 6) Compute the HMAC SHA512
            var secureHash = ComputeHmacSha512(dataToHash, hashSecret);

            // 7) Finally assemble the URL with both vnp_SecureHashType & vnp_SecureHash
            var fullQuery = new StringBuilder()
                .Append(string.Join("&", ordered))
                .Append("&vnp_SecureHashType=HmacSHA512")
                .Append("&vnp_SecureHash=").Append(secureHash)
                .ToString();

            return $"{vnpUrl}?{fullQuery}";
        }

        public bool ValidateSignature(IQueryCollection query)
        {
            var secret = _config["VnPay:HashSecret"]!;
            // 1) Re‑build the raw data** in exactly the same way **
            var dict = new SortedDictionary<string, string>();
            foreach (var key in query.Keys)
            {
                if (key.StartsWith("vnp_") &&
                    key != "vnp_SecureHash" &&
                    key != "vnp_SecureHashType")
                {
                    dict[key] = query[key];
                }
            }

            var dataToHash = string.Join("&",
                dict
                 .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

            // 2) Compute HMAC again
            var expected = ComputeHmacSha512(dataToHash, secret);
            var received = query["vnp_SecureHash"].FirstOrDefault() ?? "";

            return string.Equals(expected, received, StringComparison.OrdinalIgnoreCase);
        }

        public PlaceOrderRequest? RetrieveRequest(string txnRef)
            => _pending.TryRemove(txnRef, out var r) ? r : null;

        private static string ComputeHmacSha512(string text, string key)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
            // uppercase hex string with no dashes
            return BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();
        }
    }
}
