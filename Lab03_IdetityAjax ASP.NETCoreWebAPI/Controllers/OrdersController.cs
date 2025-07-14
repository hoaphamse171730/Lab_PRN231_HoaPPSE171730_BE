// Lab03_IdetityAjax_ASP.NETCoreWebAPI/Controllers/OrdersController.cs
using BusinessObjects.Entities;
using BusinessObjects.Models.Accounts;
using BusinessObjects.Shared;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderDAO _orderDao;
        private readonly IOrderDetailDAO _detailDao;
        private readonly IOrchidDAO _orchidDao;

        public OrdersController(
            IOrderDAO orderDao,
            IOrderDetailDAO detailDao,
            IOrchidDAO orchidDao)
        {
            _orderDao = orderDao;
            _detailDao = detailDao;
            _orchidDao = orchidDao;
        }

        // GET /api/Orders
        [HttpGet, Authorize]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.GetAccountId();
            var orders = User.IsInRole("Staff")
                ? await _orderDao.GetAllWithDetailsAsync()
                : (await _orderDao.GetAllWithDetailsAsync())
                    .Where(o => o.AccountId == userId);

            return Ok(orders);
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
    }
}
