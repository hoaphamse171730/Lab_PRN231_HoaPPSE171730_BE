// Lab03_IdetityAjax_ASP.NETCoreWebAPI/Controllers/StatsController.cs
using BusinessObjects.Shared;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Staff")]
    public class StatsController : ControllerBase
    {
        private readonly IOrderDetailDAO _detailDao;
        private readonly IOrchidDAO _orchidDao;
        private readonly IOrderDAO _orderDao;

        public StatsController(
            IOrderDetailDAO detailDao,
            IOrchidDAO orchidDao,
            IOrderDAO orderDao)
        {
            _detailDao = detailDao;
            _orchidDao = orchidDao;
            _orderDao = orderDao;
        }

        // GET api/Stats/overview
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview()
        {
            var allOrders = await _orderDao.GetAllWithDetailsAsync();
            var totalOrders = allOrders.Count();
            var totalRevenue = allOrders.Sum(o => o.TotalAmount);
            var pendingCount = allOrders.Count(o => o.OrderStatus == Constants.OrderStatusPending);
            var processingCount = allOrders.Count(o => o.OrderStatus == Constants.OrderStatusProcessing);
            var cancelledCount = allOrders.Count(o => o.OrderStatus == Constants.OrderStatusCancelled);

            return Ok(new
            {
                totalOrders,
                totalRevenue,
                pendingCount,
                processingCount,
                cancelledCount
            });
        }

        [HttpGet("top-orchids")]
        public async Task<IActionResult> TopOrchids()
        {
            // 1) all order‑details
            var details = await _detailDao.GetAllAsync();

            // 2) sum by orchidId
            var grouped = details
              .GroupBy(d => d.OrchidId)
              .Select(g => new {
                  OrchidId = g.Key,
                  Sold = g.Sum(d => d.Quantity)
              })
              .OrderByDescending(x => x.Sold)
              .Take(5)
              .ToList();

            // 3) fetch names
            var orchids = await _orchidDao.GetAllAsync();
            var map = orchids.ToDictionary(o => o.OrchidId, o => o.OrchidName);

            // 4) shape result
            var result = grouped.Select(x => new {
                orchidId = x.OrchidId,
                orchidName = map.TryGetValue(x.OrchidId, out var name) ? name : "—",
                sold = x.Sold
            });

            return Ok(result);
        }

        [HttpGet("revenue-trend")]
        public async Task<IActionResult> RevenueTrend()
        {
            var allOrders = await _orderDao.GetAllWithDetailsAsync();
            var today = DateTime.Today;
            var days = Enumerable.Range(0, 14)
                .Select(i => today.AddDays(-13 + i))
                .ToList();
            var labels = days.Select(d => d.ToString("MM-dd")).ToList();
            var values = days.Select(d =>
                allOrders.Where(o => o.OrderDate.Date == d)
                         .Sum(o => o.TotalAmount)
            ).ToList();
            return Ok(new { labels, values });
        }

        [HttpGet("order-status")]
        public async Task<IActionResult> OrderStatus()
        {
            var allOrders = await _orderDao.GetAllWithDetailsAsync();
            var statuses = new[] {
                Constants.OrderStatusPending,
                Constants.OrderStatusProcessing,
                Constants.OrderStatusCompleted,
                Constants.OrderStatusCancelled
            };
            var labels = statuses.ToList();
            var values = statuses.Select(s => allOrders.Count(o => o.OrderStatus == s)).ToList();
            return Ok(new { labels, values });
        }

        [HttpGet("orders-trend")]
        public async Task<IActionResult> OrdersTrend()
        {
            var allOrders = await _orderDao.GetAllWithDetailsAsync();
            var today = DateTime.Today;
            var days = Enumerable.Range(0, 14)
                .Select(i => today.AddDays(-13 + i))
                .ToList();
            var labels = days.Select(d => d.ToString("MM-dd")).ToList();
            var values = days.Select(d =>
                allOrders.Count(o => o.OrderDate.Date == d)
            ).ToList();
            return Ok(new { labels, values });
        }

        [HttpGet("revenue-by-category")]
        public async Task<IActionResult> RevenueByCategory()
        {
            var details = await _detailDao.GetAllAsync();
            var orchids = await _orchidDao.GetAllWithCategoryAsync();
            var catMap = orchids
                .Where(o => o.Category != null)
                .ToDictionary(o => o.OrchidId, o => o.Category!.CategoryName);
            var grouped = details
                .Where(d => catMap.ContainsKey(d.OrchidId))
                .GroupBy(d => catMap[d.OrchidId])
                .Select(g => new {
                    Category = g.Key,
                    Revenue = g.Sum(d => d.Price * d.Quantity)
                })
                .OrderByDescending(x => x.Revenue)
                .ToList();
            var labels = grouped.Select(x => x.Category).ToList();
            var values = grouped.Select(x => x.Revenue).ToList();
            return Ok(new { labels, values });
        }

        [HttpGet("top-customers")]
        public async Task<IActionResult> TopCustomers()
        {
            var allOrders = await _orderDao.GetAllWithDetailsAsync();
            var grouped = allOrders
                .GroupBy(o => o.Account)
                .Select(g => new {
                    Customer = g.Key,
                    Total = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();
            var labels = grouped.Select(x => x.Customer?.AccountName ?? "—").ToList();
            var values = grouped.Select(x => x.Total).ToList();
            return Ok(new { labels, values });
        }
    }
}
