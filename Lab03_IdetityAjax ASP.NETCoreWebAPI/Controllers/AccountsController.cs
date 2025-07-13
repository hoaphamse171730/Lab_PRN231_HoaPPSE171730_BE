using BusinessObjects.Entities;
using DataAccess.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab03_IdetityAjax_ASP.NETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountDAO _dao;
        public AccountsController(IAccountDAO dao) => _dao = dao;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _dao.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var acc = await _dao.GetByIdAsync(id);
            return acc is null ? NotFound() : Ok(acc);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Account account)
        {
            await _dao.InsertAsync(account);
            await _dao.SaveAsync();
            return CreatedAtAction(nameof(Get), new { id = account.AccountId }, account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Account account)
        {
            if (id != account.AccountId) return BadRequest();
            await _dao.UpdateAsync(account);
            await _dao.SaveAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var acc = await _dao.GetByIdAsync(id);
            if (acc is null) return NotFound();
            await _dao.DeleteAsync(acc);
            await _dao.SaveAsync();
            return NoContent();
        }
    }

}
