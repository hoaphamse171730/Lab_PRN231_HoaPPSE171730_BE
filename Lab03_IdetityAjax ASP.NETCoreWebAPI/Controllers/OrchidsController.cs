using BusinessObjects.Entities;
using BusinessObjects.Shared;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrchidsController : ControllerBase
    {
        private readonly IOrchidDAO _dao;
        public OrchidsController(IOrchidDAO dao) => _dao = dao;

        [HttpGet]
        public async Task<IActionResult> GetAll(
            int page = 1,
            int pageSize = 10,
            string? searchName = null,
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            // fetch everything with Category navigation
            var all = (await _dao.GetAllWithCategoryAsync()).AsQueryable();

            // filter by name
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                all = all.Where(o =>
                    o.OrchidName!
                     .Contains(searchName, StringComparison.OrdinalIgnoreCase)
                );
            }

            // filter by category
            if (categoryId.HasValue)
                all = all.Where(o => o.CategoryId == categoryId);

            // filter by price
            if (minPrice.HasValue)
                all = all.Where(o => o.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                all = all.Where(o => o.Price <= maxPrice.Value);

            var totalCount = all.Count();

            var items = all
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToList();

            return Ok(new { items, totalCount });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var o = await _dao.GetByIdAsync(id);
            return o is null ? NotFound() : Ok(o);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]

        public async Task<IActionResult> Create(Orchid o)
        {
            await _dao.InsertAsync(o);
            await _dao.SaveAsync();
            return CreatedAtAction(nameof(Get), new { id = o.OrchidId }, o);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]

        public async Task<IActionResult> Update(int id, Orchid o)
        {
            if (id != o.OrchidId) return BadRequest();
            await _dao.UpdateAsync(o);
            await _dao.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]

        public async Task<IActionResult> Delete(int id)
        {
            var o = await _dao.GetByIdAsync(id);
            if (o is null) return NotFound();
            await _dao.DeleteAsync(o);
            await _dao.SaveAsync();
            return NoContent();
        }
        [HttpPost("upload"), Authorize(Roles = "Staff")]
        public async Task<IActionResult> UploadImage(
            [FromServices] IStorageService storage,
            IFormFile file)
        {
            if (file == null)
                return BadRequest("No file provided.");
            var url = await storage.UploadFileAsync(file);
            return Ok(new { url });
        }

    }
}
