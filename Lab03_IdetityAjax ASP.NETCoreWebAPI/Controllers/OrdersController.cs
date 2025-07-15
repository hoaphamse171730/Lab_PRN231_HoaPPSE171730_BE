// Lab03_IdetityAjax_ASP.NETCoreWebAPI/Controllers/OrdersController.cs
using BusinessObjects.Entities;
using BusinessObjects.Models.Orders;
using BusinessObjects.Shared;
using DataAccess.Interfaces;
using DataAccess.Services;
using Lab03_IdetityAjax_ASP.NETCoreWebAPI.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderDAO _orderDao;
        private readonly IOrderDetailDAO _detailDao;
        private readonly IOrchidDAO _orchidDao;
        private readonly IHubContext<OrderNotificationHub> _hub;
        private readonly VnPayService _vnpay;

        public OrdersController(
            IOrderDAO orderDao,
            IOrderDetailDAO detailDao,
            IOrchidDAO orchidDao, 
            IHubContext<OrderNotificationHub> hub,
            VnPayService vnpay)
        {
            _orderDao = orderDao;
            _detailDao = detailDao;
            _orchidDao = orchidDao;
            _hub = hub;
            _vnpay = vnpay;
        }

        // GET /api/Orders
        public async Task<IActionResult> GetAll(
            int page = 1,
            int pageSize = 10,
            string? status = null,
            DateTime? from = null,
            DateTime? to = null)
        {
            var userId = User.GetAccountId();
            var all = await _orderDao.GetAllWithDetailsAsync();

            // 1) Role-based filter
            var filtered = all.Where(o =>
                User.IsInRole("Staff") ||
                o.AccountId == userId
            );

            // 2) Status filter
            if (!string.IsNullOrEmpty(status))
                filtered = filtered.Where(o => o.OrderStatus == status);

            // 3) Date range filter
            if (from.HasValue)
                filtered = filtered.Where(o => o.OrderDate.Date >= from.Value.Date);
            if (to.HasValue)
                filtered = filtered.Where(o => o.OrderDate.Date <= to.Value.Date);

            var list = filtered.ToList();
            var totalCount = list.Count;
            var items = list
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new { items, totalCount });
        }


        // POST /api/Orders
        [HttpPost, Authorize(Roles = "Customer")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest req)
        {
            if (req.Items == null || !req.Items.Any())
                return BadRequest("Cart is empty.");

            // 1) Create & save Order
            var order = new Order
            {
                AccountId = User.GetAccountId(),
                OrderDate = DateTime.UtcNow,
                OrderStatus = "pending",
                TotalAmount = 0m
            };
            await _orderDao.InsertAsync(order);
            await _orderDao.SaveAsync();  // now order.Id is set

            // 2) Insert OrderDetails
            decimal runningTotal = 0m;
            foreach (var item in req.Items)
            {
                var orchid = await _orchidDao.GetByIdAsync(item.OrchidId);
                if (orchid == null)
                    return BadRequest($"Orchid {item.OrchidId} not found.");

                var detail = new OrderDetail
                {
                    OrderId = order.Id,
                    OrchidId = item.OrchidId,
                    Quantity = item.Quantity,
                    Price = orchid.Price
                };
                runningTotal += orchid.Price * item.Quantity;
                await _detailDao.InsertAsync(detail);
            }
            await _detailDao.SaveAsync();

            // 3) Update Order total
            order.TotalAmount = runningTotal;
            await _orderDao.UpdateAsync(order);
            await _orderDao.SaveAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                new { order.Id, order.TotalAmount });
        }
        [HttpPut("{id}/status"), Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateStatus(int id,
            [FromBody] UpdateOrderStatusRequest req)
        {
            // 1) Validate against the four statuses
            var valid = new[]
            {
        Constants.OrderStatusPending,
        Constants.OrderStatusProcessing,
        Constants.OrderStatusCompleted,
        Constants.OrderStatusCancelled
    };
            if (!valid.Contains(req.Status))
                return BadRequest(
                    $"Invalid status. Must be one of: {string.Join(", ", valid)}");

            // 2) Load, update, save
            var order = await _orderDao.GetByIdAsync(id);
            if (order == null) return NotFound();

            order.OrderStatus = req.Status;
            await _orderDao.UpdateAsync(order);
            await _orderDao.SaveAsync();

            var groupName = $"user-{order.AccountId}";
            await _hub.Clients
                      .Group(groupName)
                      .SendAsync("OrderStatusUpdated", new
                      {
                          orderId = order.Id,
                          status = order.OrderStatus
                      });

            return NoContent();
        }

        // GET /api/Orders/{id}
        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.GetAccountId();
            var order = await _orderDao.GetByIdWithDetailsAsync(id);
            if (order == null) return NotFound();
            if (User.IsInRole("Customer") && order.AccountId != userId)
                return Forbid();
            return Ok(order);
        }

        [HttpPost("pay"), Authorize(Roles = "Customer")]
        public async Task<IActionResult> Pay([FromBody] PlaceOrderRequest req)
        {
            if (req.Items == null || !req.Items.Any())
                return BadRequest("Cart is empty.");

            decimal total = 0;
            foreach (var item in req.Items)
            {
                var o = await _orchidDao.GetByIdAsync(item.OrchidId);
                if (o == null) return BadRequest($"Orchid {item.OrchidId} not found.");
                total += o.Price * item.Quantity;
            }

            var url = _vnpay.CreatePaymentUrl(total, req);
            return Ok(new { url });
        }
        // GET /api/Orders/vnpay-return
        [HttpGet("vnpay-return")]
        [AllowAnonymous]
        public IActionResult VnPayReturn()
        {
            var q = Request.Query;
            if (!_vnpay.ValidateSignature(q))
                return BadRequest("Invalid signature");

            var code = q["vnp_ResponseCode"].ToString();
            var txn = q["vnp_TxnRef"].ToString();

            if (code == "00")
                return Redirect("https://localhost:7098/Orders/Success");
            else
                return Redirect("https://localhost:7098/Orders/Failed");
        }


    }
}
