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
    public class RoleDAO : IRoleDAO
    {
        private readonly IUOW _uow;
        private readonly IGenericRepository<Role> _repo;

        public RoleDAO(IUOW uow)
        {
            _uow = uow;
            _repo = _uow.GetRepository<Role>();
        }

        public async Task<IEnumerable<Role>> GetAllAsync() =>
            await _repo.Entities
                       .Include(r => r.Accounts)
                       .ToListAsync();

        public async Task<Role?> GetByIdAsync(int id) =>
            await _repo.Entities
                       .Include(r => r.Accounts)
                       .FirstOrDefaultAsync(r => r.RoleId == id);

        public async Task InsertAsync(Role role) =>
            await _repo.InsertAsync(role);

        public Task UpdateAsync(Role role)
        {
            _repo.Update(role);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Role role)
        {
            _repo.Delete(role);
            return Task.CompletedTask;
        }

        public async Task SaveAsync() =>
            await _uow.SaveAsync();
    }
}
