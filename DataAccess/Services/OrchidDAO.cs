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
    public class OrchidDAO : IOrchidDAO
    {
        private readonly IUOW _uow;
        private readonly IGenericRepository<Orchid> _repo;

        public OrchidDAO(IUOW uow)
        {
            _uow = uow;
            _repo = _uow.GetRepository<Orchid>();
        }

        public async Task<IEnumerable<Orchid>> GetAllAsync() =>
            await _repo.Entities
                       .Include(o => o.Category)
                       .Include(o => o.OrderDetails)
                       .ToListAsync();

        public async Task<Orchid?> GetByIdAsync(int id) =>
            await _repo.Entities
                       .Include(o => o.Category)
                       .Include(o => o.OrderDetails)
                       .FirstOrDefaultAsync(o => o.OrchidId == id);

        public async Task<IEnumerable<Orchid>> GetByNameAsync(string name) =>
            await _repo.Entities
                       .Where(o => o.OrchidName.Contains(name))
                       .ToListAsync();

        public async Task InsertAsync(Orchid orchid)
        {
            await _repo.InsertAsync(orchid);
        }

        public Task UpdateAsync(Orchid orchid)
        {
            _repo.Update(orchid);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Orchid orchid)
        {
            _repo.Delete(orchid);
            return Task.CompletedTask;
        }

        public async Task SaveAsync() =>
            await _uow.SaveAsync();
    }

}
