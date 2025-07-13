using BusinessObjects.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrchidsController : ControllerBase
    {
        private readonly IOrchidDAO _dao;
        public OrchidsController(IOrchidDAO dao) => _dao = dao;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _dao.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var o = await _dao.GetByIdAsync(id);
            return o is null ? NotFound() : Ok(o);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Orchid o)
        {
            await _dao.InsertAsync(o);
            await _dao.SaveAsync();
            return CreatedAtAction(nameof(Get), new { id = o.OrchidId }, o);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Orchid o)
        {
            if (id != o.OrchidId) return BadRequest();
            await _dao.UpdateAsync(o);
            await _dao.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var o = await _dao.GetByIdAsync(id);
            if (o is null) return NotFound();
            await _dao.DeleteAsync(o);
            await _dao.SaveAsync();
            return NoContent();
        }
    }
}
