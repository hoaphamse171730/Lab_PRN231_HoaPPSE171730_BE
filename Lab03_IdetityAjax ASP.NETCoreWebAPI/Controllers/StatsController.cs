// Lab03_IdetityAjax_ASP.NETCoreWebAPI/Controllers/StatsController.cs
using BusinessObjects.Shared;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
