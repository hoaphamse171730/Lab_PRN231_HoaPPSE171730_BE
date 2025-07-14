using BusinessObjects.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryDAO _dao;
        public CategoriesController(ICategoryDAO dao) => _dao = dao;

        // GET /api/Categories?page=1&pageSize=10
        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            var all = (await _dao.GetAllAsync()).ToList();
            var totalCount = all.Count;
            var items = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(new { items, totalCount });
        }

        // GET /api/Categories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var c = await _dao.GetByIdAsync(id);
            if (c == null) return NotFound();
            return Ok(c);
        }

        // POST /api/Categories  (Staff only)
        [HttpPost, Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create([FromBody] Category c)
        {
            await _dao.InsertAsync(c);
            await _dao.SaveAsync();
            return CreatedAtAction(nameof(GetById), new { id = c.CategoryId }, c);
        }

        // PUT /api/Categories/{id}  (Staff only)
        [HttpPut("{id}"), Authorize(Roles = "Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] Category c)
        {
            if (id != c.CategoryId) return BadRequest();
            await _dao.UpdateAsync(c);
            await _dao.SaveAsync();
            return NoContent();
        }

        // DELETE /api/Categories/{id}  (Staff only)
        [HttpDelete("{id}"), Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var c = await _dao.GetByIdAsync(id);
            if (c == null) return NotFound();
            await _dao.DeleteAsync(c);
            await _dao.SaveAsync();
            return NoContent();
        }
    }
}
