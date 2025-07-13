using BusinessObjects.Entities;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public class AccountDAO : IAccountDAO
    {
        private readonly IUOW _uow;
        private readonly IGenericRepository<Account> _repo;

        public AccountDAO(IUOW uow)
        {
            _uow = uow;
            _repo = _uow.GetRepository<Account>();
        }
        public async Task<Account?> GetByEmailAsync(string email) =>
    await _repo.Entities
               .Include(a => a.Role)
               .Include(a => a.Orders)
               .FirstOrDefaultAsync(a => a.Email == email);

        public async Task<IEnumerable<Account>> GetAllAsync() =>
            await _repo.Entities
                       .Include(a => a.Role)
                       .Include(a => a.Orders)
                       .ToListAsync();

        public async Task<Account?> GetByIdAsync(int id) =>
            await _repo.Entities
                       .Include(a => a.Role)
                       .Include(a => a.Orders)
                       .FirstOrDefaultAsync(a => a.AccountId == id);

        public async Task InsertAsync(Account account) =>
            await _repo.InsertAsync(account);

        public Task UpdateAsync(Account account)
        {
            _repo.Update(account);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Account account)
        {
            _repo.Delete(account);
            return Task.CompletedTask;
        }

        public async Task SaveAsync() =>
            await _uow.SaveAsync();
    }
}
