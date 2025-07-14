using BusinessObjects.Entities;
using BusinessObjects.Models.Accounts;
using BusinessObjects.Shared;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        // Staff can list all orders if you want:
        [HttpGet, Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _orderDao.GetAllAsync());

        // Customer places an order
        [HttpPost, Authorize(Roles = "Customer")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest req)
        {
            if (req.Items == null || !req.Items.Any())
                return BadRequest("Cart is empty.");

            // 1) Create & save the Order to get its ID
            var order = new Order
            {
                AccountId = User.GetAccountId(),
                OrderDate = DateTime.UtcNow,
                OrderStatus = "pending",
                TotalAmount = 0m     // temp, will recalc next
            };
            await _orderDao.InsertAsync(order);
            await _orderDao.SaveAsync();  // <-- now order.Id is populated

            // 2) Insert each detail
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

            // 3) Update the order’s total and save again
            order.TotalAmount = runningTotal;
            await _orderDao.UpdateAsync(order);
            await _orderDao.SaveAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = order.Id },
                new { order.Id, order.TotalAmount });
        }


        // Customer or Staff can view their order
        [HttpGet("{id}"), Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _orderDao.GetByIdAsync(id);
            if (order == null) return NotFound();
            // optionally restrict to owner if Customer:
            if (User.IsInRole("Customer") &&
                order.AccountId != User.GetAccountId())
                return Forbid();

            return Ok(order);
        }
    }

}
