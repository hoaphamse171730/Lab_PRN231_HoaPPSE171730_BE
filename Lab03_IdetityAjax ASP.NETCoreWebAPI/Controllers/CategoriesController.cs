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

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _dao.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cat = await _dao.GetByIdAsync(id);
            return cat is null ? NotFound() : Ok(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            await _dao.InsertAsync(category);
            await _dao.SaveAsync();
            return CreatedAtAction(nameof(Get), new { id = category.CategoryId }, category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (id != category.CategoryId) return BadRequest();
            await _dao.UpdateAsync(category);
            await _dao.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await _dao.GetByIdAsync(id);
            if (cat is null) return NotFound();
            await _dao.DeleteAsync(cat);
            await _dao.SaveAsync();
            return NoContent();
        }
    }
}
