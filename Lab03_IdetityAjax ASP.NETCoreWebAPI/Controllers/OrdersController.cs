using BusinessObjects.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderDAO _dao;
        public OrdersController(IOrderDAO dao) => _dao = dao;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _dao.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _dao.GetByIdAsync(id);
            return order is null ? NotFound() : Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            await _dao.InsertAsync(order);
            await _dao.SaveAsync();
            return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Order order)
        {
            if (id != order.Id) return BadRequest();
            await _dao.UpdateAsync(order);
            await _dao.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _dao.GetByIdAsync(id);
            if (order is null) return NotFound();
            await _dao.DeleteAsync(order);
            await _dao.SaveAsync();
            return NoContent();
        }
    }

}
