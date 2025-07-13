using BusinessObjects.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly IOrderDetailDAO _dao;
        public OrderDetailsController(IOrderDetailDAO dao) => _dao = dao;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _dao.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var detail = await _dao.GetByIdAsync(id);
            return detail is null ? NotFound() : Ok(detail);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderDetail detail)
        {
            await _dao.InsertAsync(detail);
            await _dao.SaveAsync();
            return CreatedAtAction(nameof(Get), new { id = detail.Id }, detail);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, OrderDetail detail)
        {
            if (id != detail.Id) return BadRequest();
            await _dao.UpdateAsync(detail);
            await _dao.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _dao.GetByIdAsync(id);
            if (detail is null) return NotFound();
            await _dao.DeleteAsync(detail);
            await _dao.SaveAsync();
            return NoContent();
        }
    }
}
