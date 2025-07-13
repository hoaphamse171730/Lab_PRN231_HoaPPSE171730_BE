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
    public class OrderDAO : IOrderDAO
    {
        private readonly IUOW _uow;
        private readonly IGenericRepository<Order> _repo;

        public OrderDAO(IUOW uow)
        {
            _uow = uow;
            _repo = _uow.GetRepository<Order>();
        }

        public async Task<IEnumerable<Order>> GetAllAsync() =>
            await _repo.Entities
                       .Include(o => o.Account)
                       .Include(o => o.OrderDetails)
                       .ToListAsync();

        public async Task<Order?> GetByIdAsync(int id) =>
            await _repo.Entities
                       .Include(o => o.Account)
                       .Include(o => o.OrderDetails)
                       .FirstOrDefaultAsync(o => o.Id == id);

        public async Task InsertAsync(Order order) =>
            await _repo.InsertAsync(order);

        public Task UpdateAsync(Order order)
        {
            _repo.Update(order);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Order order)
        {
            _repo.Delete(order);
            return Task.CompletedTask;
        }

        public async Task SaveAsync() =>
            await _uow.SaveAsync();
    }
}
