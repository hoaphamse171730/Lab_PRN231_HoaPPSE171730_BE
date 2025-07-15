using BusinessObjects.Models.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DataAccess.Services
{
    public class VnPayService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpAccessor;
        private static readonly ConcurrentDictionary<string, PlaceOrderRequest> _pending
            = new();

        public VnPayService(IConfiguration config, IHttpContextAccessor httpAccessor)
        {
            _config = config;
            _httpAccessor = httpAccessor;
        }

        public string CreatePaymentUrl(decimal amount, PlaceOrderRequest req)
        {
            // 1) store request
            var txnRef = Guid.NewGuid().ToString("N");
            _pending[txnRef] = req;

            // 2) load config
            var vnpUrl = _config["VnPay:Url"]!;
            var tmnCode = _config["VnPay:TmnCode"]!;
            var hashSecret = _config["VnPay:HashSecret"]!;
            var returnUrl = _config["VnPay:ReturnUrl"]!;

            // 3) client IP
            var ip = _httpAccessor.HttpContext?
                        .Connection.RemoteIpAddress?
                        .ToString() ?? "127.0.0.1";

            // 4) build sorted params
            var vnpParams = new SortedDictionary<string, string>
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

            // 5) percent‑encode & join
            var data = string.Join("&", vnpParams
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

            // 6) HMAC SHA512
            var hash = ComputeHmacSha512(data, hashSecret);

            // 7) build final URL (must append SecureHashType and then SecureHash)
            var builder = new StringBuilder(vnpUrl)
                .Append("?").Append(data)
                .Append("&vnp_SecureHashType=HMAC_SHA512")
                .Append("&vnp_SecureHash=").Append(hash);

            return builder.ToString();
        }

        public bool ValidateSignature(IQueryCollection query)
        {
            var secret = _config["VnPay:HashSecret"]!;
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

            var data = string.Join("&", dict
                .Select(kv =>
                    $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

            var expected = ComputeHmacSha512(data, secret);
            var received = query["vnp_SecureHash"].FirstOrDefault() ?? "";
            return string.Equals(expected, received, StringComparison.OrdinalIgnoreCase);
        }

        public PlaceOrderRequest? RetrieveRequest(string txnRef)
            => _pending.TryRemove(txnRef, out var r) ? r : null;

        private static string ComputeHmacSha512(string text, string key)
        {
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
            var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();
        }
    }
}
