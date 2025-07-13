using BusinessObjects.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleDAO _dao;
        public RolesController(IRoleDAO dao) => _dao = dao;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _dao.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var role = await _dao.GetByIdAsync(id);
            return role is null ? NotFound() : Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Role role)
        {
            await _dao.InsertAsync(role);
            await _dao.SaveAsync();
            return CreatedAtAction(nameof(Get), new { id = role.RoleId }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Role role)
        {
            if (id != role.RoleId) return BadRequest();
            await _dao.UpdateAsync(role);
            await _dao.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _dao.GetByIdAsync(id);
            if (role is null) return NotFound();
            await _dao.DeleteAsync(role);
            await _dao.SaveAsync();
            return NoContent();
        }
    }
}
