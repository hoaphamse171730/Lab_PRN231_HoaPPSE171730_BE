using BusinessObjects.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryDAO _dao;
        public CategoryController(ICategoryDAO dao) => _dao = dao;

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GetAll() =>
            Ok(await _dao.GetAllAsync());

        [HttpPost, Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create([FromBody] Category c)
        {
            await _dao.InsertAsync(c);
            await _dao.SaveAsync();
            return CreatedAtAction(nameof(Get), new { id = c.CategoryId }, c);
        }

        [HttpGet("{id}"), AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _dao.GetByIdAsync(id);
            return c is null ? NotFound() : Ok(c);
        }

        [HttpPut("{id}"), Authorize(Roles = "Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] Category c)
        {
            if (id != c.CategoryId) return BadRequest();
            await _dao.UpdateAsync(c);
            await _dao.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}"), Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _dao.GetByIdAsync(id);
            if (c is null) return NotFound();
            await _dao.DeleteAsync(c);
            await _dao.SaveAsync();
            return NoContent();
        }
    }
}
