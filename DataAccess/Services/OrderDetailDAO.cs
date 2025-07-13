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
    public class OrderDetailDAO : IOrderDetailDAO
    {
        private readonly IUOW _uow;
        private readonly IGenericRepository<OrderDetail> _repo;

        public OrderDetailDAO(IUOW uow)
        {
            _uow = uow;
            _repo = _uow.GetRepository<OrderDetail>();
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync() =>
            await _repo.Entities
                       .Include(d => d.Order)
                       .Include(d => d.Orchid)
                       .ToListAsync();

        public async Task<OrderDetail?> GetByIdAsync(int id) =>
            await _repo.Entities
                       .Include(d => d.Order)
                       .Include(d => d.Orchid)
                       .FirstOrDefaultAsync(d => d.Id == id);

        public async Task InsertAsync(OrderDetail detail) =>
            await _repo.InsertAsync(detail);

        public Task UpdateAsync(OrderDetail detail)
        {
            _repo.Update(detail);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(OrderDetail detail)
        {
            _repo.Delete(detail);
            return Task.CompletedTask;
        }

        public async Task SaveAsync() =>
            await _uow.SaveAsync();
    }
}
